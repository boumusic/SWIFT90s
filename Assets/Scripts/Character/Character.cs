﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using TMPro;
using Mirror;

public class Character : MonoBehaviour
{
    #region Fields

    [Header("Gameplay")]
    public Rigidbody body;
    public CharacterSettings m;
    public Propeller p;
    public Collider defaultCollider;
    public GameObject dodgeCollider;
    public NetworkedPlayer player;

    [Header("Visuals")]
    public CharacterAnimator animator;
    public GameObject visuals;
    public CharacterFeedbacks fb;
    public Texture2D[] characterTextures;
    public Flag flagVisuals;
    public Renderer[] rends;
    public TextMeshPro textName;

    [Header("VFX")]
    public ParticleSystem wallSlideFx;
    public GameObject flagBearerFx;

    [Header("Debug")]
    public bool receiveDebugInput = false;
    public bool drawAttackHitbox;
    public bool drawMovementHitbox;

    #region State
    private StateMachine<CharacterState> stateMachine;
    public CharacterState CurrentState => stateMachine != null ? stateMachine.State : CharacterState.Grounded;
    #endregion

    #region Movement

    #region Velocity

    private Vector3 velocity;
    public Vector3 Velocity => velocity;
    private float xAccel = 0;
    public float XAccel => xAccel;


    #endregion

    #region Input

    private float horizontalAxis = 0;
    private float verticalAxis = 0;
    public float HorizontalAxis { get => horizontalAxis; }
    public float VerticalAxis { get => verticalAxis; }

    #endregion

    #endregion

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
    }

    private float initialZ;
    public void Initialize()
    {
        SwitchDodgeCollider(false);
        stateMachine = StateMachine<CharacterState>.Initialize(this);
        stateMachine.ManualUpdate = true;
        stateMachine.ChangeState(CharacterState.Falling);
        ResetJumpCount();

        flagVisuals.Initialize(1 - TeamIndex);
        UpdateFlagVisuals();
        UpdateTexture();
        UpdateTextName();
        initialZ = body.position.z;
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void Update()
    {
        stateMachine.UpdateManually();
        DebugInput();
        CalculateHorizontalAcceleration();
        CalculateHorizontalVelocity();
        OrientModelToDirection();
        AttackUpdate();
        DodgeCooldown();
        p.FeedInputs(new Vector2(horizontalAxis, verticalAxis));
        WallJumpUpdate();
        animator.Run(Mathf.Abs(velocity.x) >= 0.01f && grounded);
        body.position = new Vector3(body.position.x, body.position.y, initialZ);

        if (hitWall.collider != null) lastHitwall = hitWall;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
        {
            Die();
        }
#endif        
    }

    private void OnDrawGizmos()
    {
        DrawBox();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up * m.castWallHeight, transform.position + Vector3.up * m.castWallHeight + castWallDirection.normalized * m.castWallLength);
    }

    #endregion

    #region Input
    private bool nullInput => horizontalAxis == 0 && verticalAxis == 0;
    private bool nullVelocity => body.velocity == Vector3.zero;

    public bool DownButton { get; private set; }

    private void DebugInput()
    {
        if (receiveDebugInput)
        {
            InputHorizontal(Input.GetAxisRaw("Horizontal"));
            InputVertical(Input.GetAxisRaw("Vertical"));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ReceiveJumpInput();
            }

            InputDownButton(Input.GetKey(KeyCode.S));
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Dodge();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartAttack();
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                Taunt();
            }
        }
    }

    public void InputHorizontal(float horizontal)
    {
        if (!taunting)
            this.horizontalAxis = Mathf.Abs(horizontal) < 0.2f ? 0 : Mathf.Sign(horizontal);
    }

    public void InputVertical(float vertical)
    {
        if (!taunting)
        {
            this.verticalAxis = Mathf.Abs(vertical) < 0.2f ? 0 : Mathf.Sign(vertical);
            InputDownButton(vertical < 0f);
        }
    }

    public void InputDownButton(bool down)
    {
        DownButton = down;
    }

    #endregion

    #region Movement

    private bool isPropelling => p.IsPropelling;

    public float CurrentAcceleration
    {
        get
        {
            float accel = grounded ? m.groundedAcceleration : m.aerialAcceleration;
            if (isPropelling)
            {
                accel *= p.Current().airControl;
            }

            return accel;
        }
    }

    public float CurrentDeceleration
    {
        get
        {
            float decel = grounded ? m.groundedDeceleration : m.aerialDeceleration;
            if (isPropelling)
            {
                decel *= 1;
            }
            return decel;
        }
    }

    private void CalculateHorizontalAcceleration()
    {
        if (horizontalAxis != 0)
        {
            xAccel += horizontalAxis * CurrentAcceleration * Time.deltaTime;
        }

        else
        {
            xAccel /= CurrentDeceleration;
        }

        xAccel = Mathf.Clamp(xAccel, -1f, 1f);
    }

    private void SnapAccelToAxis()
    {
        xAccel = horizontalAxis;
    }

    private void CalculateHorizontalVelocity()
    {
        SetHorizontalVelocity(xAccel * m.runSpeed);
    }

    private void SetVerticalVelocity(float y)
    {
        velocity = new Vector3(velocity.x, y, 0f);
    }

    private void SetHorizontalVelocity(float x)
    {
        velocity = new Vector3(x, velocity.y, 0f);
    }

    private void ApplyVelocity()
    {
        if (p.IsPropelling)
        {
            Propulsion current = p.Current();
            float strength = 1 - current.CurrentStrengthHoriz;
            //Vector3 horizontal = Vector3.Lerp(p.Velocity(), velocity,  1 -current.CurrentStrengthHoriz);
            Vector3 horizontal = p.Velocity() + velocity * Mathf.Clamp01(strength) * current.airControl;
            body.velocity = new Vector3(horizontal.x, p.Velocity().y, horizontal.z);
        }
        else
        {
            if (!taunting)
                body.velocity = velocity;
        }
    }

    #endregion

    #region Grounded

    private bool grounded => CurrentState == CharacterState.Grounded;

    private void Grounded_Enter()
    {
        //Debug.Log("Enter Ground");
        CanPassThrough(false);
        SetVerticalVelocity(0);
        ResetJumpCount();
    }

    private void Grounded_Update()
    {
        if (!CastGround())
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }

        else
        {
            if (DownButton && WalkingOnPassThroughPlatform)
            {
                CanPassThrough(true);
                stateMachine.ChangeState(CharacterState.Falling);
            }
        }
    }

    #endregion

    #region Jump

    private int jumpLeft = 0;
    private float jumpProgress = 0f;

    public void ReceiveJumpInput()
    {
        if (taunting) return;
        if (isWallSliding)
        {
            WallJump();
        }

        else
        {
            if (jumpLeft > 0)
            {
                stateMachine.ChangeState(CharacterState.Jumping);
            }
        }
    }

    private void Jumping_Enter()
    {
        jumpLeft--;
        animator.Jump(jumpLeft == 0);
        jumpProgress = 0f;
        SnapAccelToAxis();
        CanPassThrough(true);
    }

    private void Jumping_Update()
    {
        jumpProgress += Time.deltaTime / m.jumpDuration;
        float strength = m.jumpCurve.Evaluate(jumpProgress) * m.jumpStrength;
        SetVerticalVelocity(strength);


        if (jumpProgress > 1f || CastCeiling())
        {
            SetVerticalVelocity(0);
            stateMachine.ChangeState(CharacterState.Falling);
        }

    }

    private void ResetJumpCount()
    {
        jumpLeft = m.jumpCount;
    }

    #endregion

    #region Fall

    private float yVelocityStartFall = 0f;
    public float FallProgress { get; private set; }

    private void ResetFallProgress()
    {
        FallProgress = 0f;
    }

    private void Falling_Enter()
    {
        ResetFallProgress();
        yVelocityStartFall = velocity.y;
        animator.Falling(true);
    }

    private void Falling_Update()
    {
        if (!p.IsPropelling)
        {
            if (CastGround())
            {
                bool land = false;
                if (WalkingOnPassThroughPlatform)
                {
                    if (DownButton)
                    {
                        CanPassThrough(true);
                    }

                    else
                    {
                        land = true;
                    }
                }

                else
                {
                    land = true;
                }

                if (land)
                {
                    CanPassThrough(false);
                    SnapToGround();
                    Land();
                    stateMachine.ChangeState(CharacterState.Grounded);
                }
            }

            FallProgress += Time.deltaTime / m.timeToReachMaxFall;
            FallProgress = Mathf.Clamp01(FallProgress);

            float mul = DownButton ? 2 : 1;

            float fall = Mathf.Lerp(yVelocityStartFall, -m.maxFallSpeed * mul, m.fallCurve.Evaluate(FallProgress));
            SetVerticalVelocity(fall);

            if (CastWall())
            {
                stateMachine.ChangeState(CharacterState.WallSliding);
            }
        }
    }

    private void Falling_Exit()
    {
        animator.Falling(false);
        FallProgress = 0f;
    }

    #endregion

    private void Land()
    {
        fb.Play("Land");
        animator.Land();
    }

    #region WallSliding

    private bool isWallSliding => CurrentState == CharacterState.WallSliding;
    private float wallSlidingProgress = 0f;
    private float initialVelocity;
    private Coroutine leavingWallSlide;

    private void WallSliding_Enter()
    {
        initialVelocity = body.velocity.y;
        animator.WallSliding(true);
        wallSlidingProgress = 0f;
        SetVerticalVelocity(-m.minWallSlideSpeed);
        SetHorizontalVelocity(0);
        wallSlideFx.Play();
        //FlipVisuals(true);
    }

    private void WallSliding_Update()
    {
        wallSlidingProgress += Time.deltaTime / m.timeToReachMaxSlide;
        wallSlidingProgress = Mathf.Clamp01(wallSlidingProgress);

        float slide = Mathf.Lerp(-m.minWallSlideSpeed, -m.maxWallSlideSpeed, m.slideCurve.Evaluate(wallSlidingProgress));
        SetVerticalVelocity(slide);
        SetHorizontalVelocity(0);

        if (!CastWall() && leavingWallSlide != null)
        {
            leavingWallSlide = StartCoroutine(LeavingWallSlide());
        }

        if (CastWall())
        {
            if (leavingWallSlide != null) StopCoroutine(LeavingWallSlide());
        }

        if (CastGround())
        {
            Land();
            if (leavingWallSlide != null) StopCoroutine(LeavingWallSlide());
            SnapToGround();
            stateMachine.ChangeState(CharacterState.Grounded);

        }
    }

    private IEnumerator LeavingWallSlide()
    {
        yield return new WaitForSeconds(m.wallSlideBuffer);
        stateMachine.ChangeState(CharacterState.Falling);
    }

    private void WallSliding_Exit()
    {
        animator.WallSliding(false);
        wallSlideFx.Stop();
        //FlipVisuals(false);
    }

    #endregion

    #region WallJump

    private bool wallJumping = false;

    private void WallJump()
    {
        stateMachine.ChangeState(CharacterState.Falling);
        Debug.Log("WallJump " + lastHitwall.collider.gameObject.name);
        ResetFallProgress();

        Vector3 jumpDir = (lastHitwall.normal + Vector3.up).normalized;
        Vector3 noZ = new Vector3(jumpDir.x, jumpDir.y, 0);
        p.RegisterPropulsion(noZ, m.wallJump, EndWallJump);
        animator.WallJump();
        wallJumping = true;
    }

    private void WallJumpUpdate()
    {
        //if (wallJumping)
        //FlipVisuals(true);
    }

    private void EndWallJump()
    {
        wallJumping = false;
        //FlipVisuals(false);
    }

    #endregion

    #region Attack

    private Vector3 attackDirection => nullInput ? nullVelocity ? ForwardNoZ : body.velocity.normalized : new Vector3(horizontalAxis, verticalAxis, 0).normalized;
    private Vector3 lastAttackDirection;
    public bool IsAttacking { get; private set; }
    private bool CanAttack => !IsDodging && attackCooldownDone && !HasFlag && !taunting;
    private float attackProgress = 0f;
    private float attackCooldownProgress = 0f;
    private bool attackCooldownDone = true;

    public void StartAttack()
    {
        if (CanAttack)
        {
            charactersHitThisAttack.Clear();

            lastAttackDirection = attackDirection;
            IsAttacking = true;
            attackProgress = 0f;
            attackCooldownDone = false;
            attackCooldownProgress = 0f;
            p.RegisterPropulsion(lastAttackDirection, m.attackImpulse);
            animator.Attacking(true);
            Vector2 dir = new Vector2(Mathf.Abs(lastAttackDirection.x) > 0f ? 1 : 0f, lastAttackDirection.y == 0 ? 0 : Mathf.Sign(lastAttackDirection.y));
            animator.AttackDirection(dir);
            Debug.Log("Attack Dir: " + dir);
        }
    }

    List<Character> charactersHitThisAttack = new List<Character>();
    private void AttackUpdate()
    {
        if (IsAttacking)
        {
            hitsAttack = Physics.BoxCastAll(AttackOrigin, AttackBox, lastAttackDirection, Quaternion.identity, m.attackLength);

            if (hitsAttack.Length > 0)
            {
                for (int i = 0; i < hitsAttack.Length; i++)
                {
                    Character chara;
                    if (hitsAttack[i].collider.TryGetComponent(out chara))
                    {
                        if (chara != this && chara.TeamIndex != TeamIndex)
                        {
                            if (!chara.IsDodging && !IsDead && !charactersHitThisAttack.Contains(chara))
                            {
                                charactersHitThisAttack.Add(chara);

                                AudioManager.instance.PlaySound(AudioManager.instance.AS_Fight, AudioManager.instance.AC_Hit);
                                AudioManager.instance.PlaySoundRandomPitch(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_Kill);
                                player.CmdKillPlayer(player.netIdentity, chara.GetComponent<NetworkIdentity>());
                            }
                        }
                    }
                }

            }

            attackProgress += Time.deltaTime / m.attackDuration;
            if (attackProgress >= 1)
            {
                if (IsAttacking)
                {
                    ResetFallProgress();
                }
                animator.Attacking(false);
                IsAttacking = false;
            }
        }

        else
        {
            if (!attackCooldownDone)
            {
                attackCooldownProgress += Time.deltaTime / m.attackCooldown;
                if (attackCooldownProgress >= 1f)
                {
                    attackCooldownProgress = 1f;
                    attackCooldownDone = true;
                }
            }
        }
    }

    public void Kill(Character chara)
    {
        if (!chara.IsDead)
        {
            if (chara.HasFlag)
            {
                UIManager.Instance.LogMessage(PlayerName + " retrieved the flag from " + chara.PlayerName);
            }
            UIManager.Instance.DisplayKillFeed(this, chara);
            chara.Die();
            //chara.gameObject.SetActive(false);
            fb.Play("Kill");

            //Debug.Log("Hit " + chara.gameObject.name);
        }
    }

    #endregion

    #region Cast

    public Vector3 FeetOrigin => transform.position + Vector3.up * m.castGroundOrigin;
    public Vector3 HeadOrigin => transform.position + Vector3.up * m.castCeilingOrigin;
    public Vector3 AttackOrigin => transform.position + Vector3.up * m.attackOrigin + (IsAttacking ? lastAttackDirection : attackDirection) * m.attackOffset;
    public Vector3 CastBox => new Vector3(m.castBoxWidth, 0, m.castBoxWidth);
    public Vector3 AttackBox => new Vector3(m.attackWidth, m.attackWidth, m.attackWidth);
    private RaycastHit[] hitGrounds;
    private RaycastHit hitCeiling;
    private RaycastHit[] hitsAttack;
    private RaycastHit hitWall;
    private RaycastHit lastHitwall;
    
    private void DrawBox()
    {
        if (drawMovementHitbox)
        {
            BoxCastDrawer.DrawBoxCastBox(FeetOrigin, CastBox * m.groundCastRadius, Quaternion.identity, -Vector3.up, m.groundRaycastDown, Color.red);
            BoxCastDrawer.DrawBoxCastBox(HeadOrigin, CastBox * m.groundCastRadius, Quaternion.identity, Vector3.up, m.castCeilingLength, Color.yellow);
        }

        if (drawAttackHitbox)
        {
            Color col = IsAttacking ? Color.cyan : Color.blue;
            Vector3 dir = IsAttacking ? lastAttackDirection : attackDirection;
            BoxCastDrawer.DrawBoxCastBox(AttackOrigin, AttackBox, Quaternion.identity, dir, m.attackLength, col);
        }
    }

    public bool CastCeiling()
    {
        if (Physics.BoxCast(HeadOrigin, CastBox * m.groundCastRadius, Vector3.up, out hitCeiling, Quaternion.identity, m.castCeilingLength, m.wallMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public bool CastGround()
    {
        hitGrounds = Physics.BoxCastAll(FeetOrigin, CastBox * m.groundCastRadius, -Vector3.up, Quaternion.identity, m.groundRaycastDown, m.groundMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hitGrounds.Length; i++)
        {
            //Debug.Log(hitGrounds[i].collider.gameObject.name);
        }
        return hitGrounds.Length > 0;
    }

    private Vector3 castWallDirection => horizontalAxis != 0 ? new Vector3(horizontalAxis, 0, 0).normalized : body.velocity.x != 0 ? new Vector3(body.velocity.x, 0, 0) : ForwardNoZ;

    public bool CastWall()
    {
        if (Physics.Raycast(transform.position + Vector3.up * m.castWallHeight, castWallDirection.normalized, out hitWall, m.castWallLength * Mathf.Abs(horizontalAxis), m.wallMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return false;
    }

    private void SnapToGround()
    {
        body.position = new Vector3(body.position.x, hitGrounds[0].point.y, body.position.z);
    }

    #endregion

    #region Layer

    public bool WalkingOnPassThroughPlatform => hitGrounds != null ? (hitGrounds.Length > 0 ? hitGrounds[0].collider.gameObject.layer == m.passThroughLayerPlatform : false) : false;
    private bool canPassThrough => gameObject.layer == m.passThroughLayer;
    private void CanPassThrough(bool pass)
    {
        gameObject.layer = pass ? m.passThroughLayer : m.defaultLayer;
    }

    #endregion

    #region Dodge

    private Vector3 ForwardNoZ => new Vector3(transform.forward.x, transform.forward.y, 0).normalized;
    private Vector3 dodgeDirection => nullInput ? ForwardNoZ : new Vector3(horizontalAxis, verticalAxis, 0).normalized;
    private bool dodgeCooldownDone = true;
    private float dodgeCooldownProgress = 0f;
    private bool canDodge => dodgeCooldownDone && !IsAttacking;

    public bool IsDodging { get; private set; }
    private bool shouldResetFall = false;

    public void Dodge()
    {
        if (canDodge)
        {
            dodgeCooldownDone = false;
            dodgeCooldownProgress = 0f;
            if (verticalAxis >= 0) shouldResetFall = true;
            IsDodging = true;
            animator.Dodge();
            Vector3 dir = dodgeDirection;
            p.RegisterPropulsion(dir, m.dodge, EndDodge);
            SwitchDodgeCollider(true);
        }
    }

    private void EndDodge()
    {
        SwitchDodgeCollider(false);
        IsDodging = false;
        if (shouldResetFall)
            ResetFallProgress();
    }

    private void DodgeCooldown()
    {
        if (!IsDodging && !dodgeCooldownDone)
        {
            dodgeCooldownProgress += Time.deltaTime / m.dodgeCooldown;
            if (dodgeCooldownProgress >= 1)
            {
                dodgeCooldownProgress = 1f;
                dodgeCooldownDone = true;
            }
        }
    }

    private void SwitchDodgeCollider(bool dodging)
    {
        defaultCollider.enabled = !dodging;
        dodgeCollider.SetActive(dodging);
    }

    #endregion

    #region Visuals

    private float lastDir = 1;

    private void OrientModelToDirection()
    {
        if (horizontalAxis == 0) lastDir = Mathf.Sign(velocity.x);
        else
            lastDir = Mathf.Sign(velocity.x);

        if (!wallJumping)
        {
            float targetAngle = lastDir * 90f;
            Quaternion target = Quaternion.AngleAxis(targetAngle, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, 0.1f);
        }
    }

    private void UpdateFlagVisuals()
    {
        flagVisuals.gameObject.SetActive(HasFlag);
    }

    public void UpdateTextName()
    {
        textName.text = PlayerName;
    }

    #endregion

    #region Taunt

    private bool CanTaunt => grounded;
    private bool taunting => CurrentState == CharacterState.Taunting;

    public void Taunt()
    {
        if (CanTaunt)
        {
            stateMachine.ChangeState(CharacterState.Taunting);
        }
    }

    private void Taunting_Enter()
    {
        SetHorizontalVelocity(0);
        SetVerticalVelocity(0);
        animator.Taunt();
        animator.Taunting(true);
        StartCoroutine(TauntDuration());
    }

    private IEnumerator TauntDuration()
    {
        yield return new WaitForSeconds(2);
        EndTaunt();
    }

    private void EndTaunt()
    {
        animator.Taunting(false);
        stateMachine.ChangeState(CharacterState.Grounded);
    }

    #endregion

    #region Team

    public Color TeamColor => TeamManager.Instance.GetTeamColor(TeamIndex);
    public int TeamIndex => player.TeamIndex;

    public string PlayerName => player.Username;

    public void UpdateTexture()
    {
        Material mat = new Material(rends[0].material);
        mat.SetTexture("_MainTex", characterTextures[TeamIndex]);
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sharedMaterial = mat;
        }
    }

    #endregion

    #region Flag

    public bool HasFlag { get; private set; }
    private Altar capturedAltar;

    public void CaptureFlag(Altar altar)
    {
        //SAME TEAM
        if (UIManager.Instance.Player.TeamIndex == TeamIndex)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_FlagTookAlly);
        }

        else
        {
            AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_FlagTookEnemy);
        }

        capturedAltar = altar;
        HasFlag = true;
        UpdateFlagVisuals();

        flagBearerFx.SetActive(HasFlag);
    }

    public void DropFlag()
    {
        if (capturedAltar != null)
        {
            CTFManager.Instance.RetrievedFlagOfTeam(capturedAltar.teamIndex);
        }
        capturedAltar?.ResetFlag();
        capturedAltar = null;
        HasFlag = false;

        flagBearerFx.SetActive(HasFlag);
        UpdateFlagVisuals();
    }

    public void Score()
    {
        //SAME TEAM
        if (UIManager.Instance.Player.TeamIndex == TeamIndex)
        {
            if (player.Team.Score == 0)
                AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_ScoreAlly_01);
            if (player.Team.Score == 1)
                AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_ScoreAlly_02);
            if (player.Team.Score == 2)
            {
                AudioManager.instance.MuteAudioSource(AudioManager.instance.AS_Loop, true);
                AudioManager.instance.MuteAudioSource(AudioManager.instance.AS_Movement, true);
                AudioManager.instance.MuteAudioSource(AudioManager.instance.AS_Fight, true);
                AudioManager.instance.FadeOut(AudioManager.instance.AS_Music, 0.6f);
                AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_ScoreAlly_03);
            }
        }

        //ENEMY TEAM
        else
        {
            if (player.Team.Score == 0)
                AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_ScoreEnemy_01);
            if (player.Team.Score == 1)
                AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_ScoreEnemy_02);
            if (player.Team.Score == 2)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_ScoreEnemy_03);
                AudioManager.instance.MuteAudioSource(AudioManager.instance.AS_Loop, true);
                AudioManager.instance.MuteAudioSource(AudioManager.instance.AS_Movement, true);
                AudioManager.instance.MuteAudioSource(AudioManager.instance.AS_Fight, true);
                AudioManager.instance.FadeOut(AudioManager.instance.AS_Music, 0.6f);
            }
        }
    }

    #endregion

    #region Death

    public bool IsDead { get; private set; }
    public void Die()
    {
        if (!IsDead)
        {
            visuals.gameObject.SetActive(false);
            IsDead = true;
            fb.Play("Death");
            DropFlag();
            AudioManager.instance.PlaySoundRandomPitch(AudioManager.instance.AS_Feedback, AudioManager.instance.AC_Death);

            defaultCollider.enabled = false;

            StartCoroutine(DeathTimerRoutine());
        }
    }

    IEnumerator DeathTimerRoutine()
    {
        yield return new WaitForSecondsRealtime(CTFManager.Instance.respawnTimer);

        transform.position = player.spawnPosition;

        visuals.gameObject.SetActive(true);
        IsDead = false;
        defaultCollider.enabled = true;
    }

    #endregion
}

public enum CharacterState
{
    Grounded,
    Jumping,
    Falling,
    WallClimbing,
    WallSliding,
    Taunting
}
