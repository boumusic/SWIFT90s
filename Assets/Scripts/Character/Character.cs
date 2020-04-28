using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class Character : MonoBehaviour
{
    #region Fields

    [Header("Components")]
    public Rigidbody body;
    public CharacterSettings m;
    public Propeller p;
    public Animator visuals;
    public CharacterAnimator animator;

    [Header("Debug")]
    public bool receiveDebugInput = true;

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
        stateMachine = StateMachine<CharacterState>.Initialize(this);
        stateMachine.ManualUpdate = true;
        stateMachine.ChangeState(CharacterState.Falling);
        ResetJumpCount();
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
        p.FeedInputs(new Vector2(horizontalAxis, verticalAxis));

        animator.Run(Mathf.Abs(velocity.x) >= 0.01f && grounded);
    }

    private void OnDrawGizmos()
    {
        DrawBox();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + Vector3.up * 1.5f + transform.forward * m.castWallLength * Mathf.Abs(horizontalAxis));
    }

    #endregion

    #region Input
    private bool nullInput => horizontalAxis == 0 && verticalAxis == 0;
    private bool nullVelocity => body.velocity == Vector3.zero;
    private Vector3 actionDirection => nullInput ? nullVelocity ? transform.forward : body.velocity.normalized : new Vector3(horizontalAxis, verticalAxis, 0).normalized;

    private bool downButton = false;

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
        }
    }

    public void InputHorizontal(float horizontal)
    {
        this.horizontalAxis = Mathf.Abs(horizontal) < 0.2f ? 0 : Mathf.Sign(horizontal);
    }

    public void InputVertical(float vertical)
    {
        this.verticalAxis = Mathf.Abs(vertical) < 0.2f ? 0 : Mathf.Sign(vertical);
    }

    public void InputDownButton(bool down)
    {
        downButton = down;
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
            Vector3 horizontal = p.Velocity() + velocity * Mathf.Clamp01(strength);
            body.velocity = new Vector3(horizontal.x, p.Velocity().y, horizontal.z);
        }
        else
        {
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
        animator.Land();
    }

    private void Grounded_Update()
    {
        if (!CastGround())
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }

        else
        {
            if (downButton && walkingOnPassThroughPlatform)
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


        if (jumpProgress > 1f)
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
    private float fallProgress = 0f;

    private void ResetFallProgress()
    {
        fallProgress = 0f;
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
                if (walkingOnPassThroughPlatform)
                {
                    if (downButton)
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
                    stateMachine.ChangeState(CharacterState.Grounded);
                }
            }

            fallProgress += Time.deltaTime / m.timeToReachMaxFall;
            fallProgress = Mathf.Clamp01(fallProgress);

            float mul = downButton ? 2 : 1;

            float fall = Mathf.Lerp(yVelocityStartFall, -m.maxFallSpeed * mul, m.fallCurve.Evaluate(fallProgress));
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
    }

    #endregion

    #region WallSliding

    private bool isWallSliding => CurrentState == CharacterState.WallSliding;
    private float wallSlidingProgress = 0f;

    private void WallSliding_Enter()
    {
        animator.WallSliding(true);
        wallSlidingProgress = 0f;
        SetVerticalVelocity(-m.minWallSlideSpeed);
        SetHorizontalVelocity(0);
    }

    private void WallSliding_Update()
    {
        wallSlidingProgress += Time.deltaTime / m.timeToReachMaxSlide;
        wallSlidingProgress = Mathf.Clamp01(wallSlidingProgress);

        float slide = Mathf.Lerp(-m.minWallSlideSpeed, -m.maxWallSlideSpeed, m.slideCurve.Evaluate(wallSlidingProgress));
        SetVerticalVelocity(slide);
        SetHorizontalVelocity(0);

        if (!CastWall())
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }

        if (CastGround())
        {
            SnapToGround();
            stateMachine.ChangeState(CharacterState.Grounded);
        }
    }

    private void WallSliding_Exit()
    {
        animator.WallSliding(false);
    }

    #endregion

    #region WallJump

    private void WallJump()
    {
        Vector3 jumpDir = (hitWall.normal + Vector3.up).normalized;
        p.RegisterPropulsion(jumpDir, m.wallJump);
        animator.WallJump();
    }

    #endregion

    #region Attack

    public bool IsAttacking { get; private set; }
    private float attackProgress = 0f;

    private void StartAttack()
    {
        IsAttacking = true;
        attackProgress = 0f;
    }

    private void AttackUpdate()
    {
        
        if (IsAttacking)
        {
            hitsAttack = Physics.BoxCastAll(AttackOrigin, AttackBox, actionDirection, Quaternion.identity, m.attackLength);

            if(hitsAttack.Length > 0)
            {
                for (int i = 0; i < hitsAttack.Length; i++)
                {
                    Debug.Log("Hit " + hitsAttack[i].collider.gameObject.name);
                    Character chara;
                    if (hitsAttack[i].collider.TryGetComponent(out chara))
                    {
                        if(chara != this)
                        {
                            Debug.Log("Hit " + chara.gameObject.name);
                            chara.gameObject.SetActive(false);
                        }                       
                    }
                }
                
            }

            attackProgress += Time.deltaTime / m.attackDuration;
            if (attackProgress >= 1)
            {
                IsAttacking = false;
            }
        }
    }

    #endregion

    #region Cast

    public Vector3 FeetOrigin => transform.position + Vector3.up * m.castGroundOrigin;
    public Vector3 HeadOrigin => transform.position + Vector3.up * m.castCeilingOrigin;
    public Vector3 AttackOrigin => transform.position + Vector3.up * m.attackOrigin + actionDirection * m.attackOffset;
    public Vector3 CastBox => new Vector3(m.castBoxWidth, 0, m.castBoxWidth);
    public Vector3 AttackBox => new Vector3(m.attackWidth, m.attackWidth, m.attackWidth);
    private RaycastHit hitGround;
    private RaycastHit hitCeiling;
    private RaycastHit[] hitsAttack;
    private RaycastHit hitWall;

    private void DrawBox()
    {
        BoxCastDrawer.DrawBoxCastBox(FeetOrigin, CastBox * m.groundCastRadius, Quaternion.identity, -Vector3.up, m.groundRaycastDown, Color.red);
        BoxCastDrawer.DrawBoxCastBox(HeadOrigin, CastBox * m.groundCastRadius, Quaternion.identity, Vector3.up, m.castCeilingLength, Color.yellow);
        BoxCastDrawer.DrawBoxCastBox(AttackOrigin, AttackBox, Quaternion.identity, actionDirection, m.attackLength, Color.blue);
    }

    public bool CastCeiling()
    {
        if (Physics.BoxCast(HeadOrigin, CastBox * m.groundCastRadius, Vector3.up, out hitCeiling, Quaternion.identity, m.castCeilingLength, m.groundMask))
        {
            return true;
        }
        return false;
    }

    public bool CastGround()
    {
        if (Physics.BoxCast(FeetOrigin, CastBox * m.groundCastRadius, -Vector3.up, out hitGround, Quaternion.identity, m.groundRaycastDown, m.groundMask))
        {
            return true;
        }
        return false;
    }

    public bool CastWall()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, out hitWall, m.castWallLength * Mathf.Abs(horizontalAxis), m.groundMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return false;
    }

    private void SnapToGround()
    {
        body.position = new Vector3(body.position.x, hitGround.point.y, body.position.z);
    }

    #endregion

    #region Layer

    private bool walkingOnPassThroughPlatform => hitGround.collider != null ? hitGround.collider.gameObject.layer == m.passThroughLayer : false;
    private bool canPassThrough => gameObject.layer == m.passThroughLayer;
    private void CanPassThrough(bool pass)
    {
        gameObject.layer = pass ? m.passThroughLayer : m.defaultLayer;
    }

    #endregion

    #region Dodge

    public void Dodge()
    {
        animator.Dodge();
        Vector3 dir = actionDirection;
        p.RegisterPropulsion(dir, m.dodge, EndDodge);
    }

    private void EndDodge()
    {
        ResetFallProgress();
    }

    #endregion

    #region Visuals

    private float lastDir = 1;

    private void OrientModelToDirection()
    {
        if (horizontalAxis == 0) lastDir = Mathf.Sign(velocity.x);
        else
            lastDir = Mathf.Sign(velocity.x);
        transform.forward = new Vector3(lastDir, 0, 0);
    }

    #endregion
}

public enum CharacterState
{
    Grounded,
    Jumping,
    Falling,
    WallClimbing,
    WallSliding
}
