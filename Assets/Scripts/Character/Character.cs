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
    public float HorizontalAxis { get => horizontalAxis; }

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

    private bool downButton = false;

    private void DebugInput()
    {
        InputHorizontal(Input.GetAxisRaw("Horizontal"));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartJump();
        }

        InputDownButton(Input.GetKey(KeyCode.S));
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            Dodge();
        }
    }

    public void InputHorizontal(float horizontal)
    {
        this.horizontalAxis = Mathf.Abs(horizontal) < 0.2f ? 0 : Mathf.Sign(horizontal);
    }

    public void InputDownButton(bool down)
    {
        downButton = down;
    }

    #endregion

    #region Movement

    public float CurrentAcceleration => grounded ? m.groundedAcceleration : m.aerialAcceleration;
    public float CurrentDeceleration => grounded ? m.groundedDeceleration : m.aerialDeceleration;

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
        if (p.IsPropelling) body.velocity = p.Velocity();
        else body.velocity = velocity;
    }

    #endregion

    #region Grounded

    private bool grounded => CurrentState == CharacterState.Grounded;

    private void Grounded_Enter()
    {
        Debug.Log("Enter Ground");
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
            if(downButton && walkingOnPassThroughPlatform)
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

    public void StartJump()
    {
        if (jumpLeft > 0)
        {
            stateMachine.ChangeState(CharacterState.Jumping);
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

    private void Falling_Enter()
    {
        fallProgress = 0f;
        yVelocityStartFall = velocity.y;
    }

    private void Falling_Update()
    {
        if(!p.IsPropelling)
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
            float fall = Mathf.Lerp(yVelocityStartFall, -m.maxFallSpeed, m.fallCurve.Evaluate(fallProgress));
            SetVerticalVelocity(fall);

            if (CastWall())
            {
                stateMachine.ChangeState(CharacterState.WallSliding);
            }
        }
    }

    #endregion

    #region WallSliding

    private float wallSlidingProgress = 0f;
    private void WallSliding_Enter()
    {
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

    #endregion

    #region Cast

    public Vector3 FeetOrigin => transform.position + Vector3.up * m.castGroundOrigin;
    public Vector3 HeadOrigin => transform.position + Vector3.up * m.castCeilingOrigin;
    public Vector3 CastBox => new Vector3(m.castBoxWidth, 0, m.castBoxWidth);
    private RaycastHit hitGround;
    private RaycastHit hitCeiling;
    private RaycastHit hitWall;

    private void DrawBox()
    {
        BoxCastDrawer.DrawBoxCastBox(FeetOrigin, CastBox * m.groundCastRadius, Quaternion.identity, -Vector3.up, m.groundRaycastDown, Color.red);
        BoxCastDrawer.DrawBoxCastBox(HeadOrigin, CastBox * m.groundCastRadius, Quaternion.identity, Vector3.up, m.castCeilingLength, Color.yellow);
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
    private Vector3 mousePos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Vector3 mouseDir => (mousePos - transform.position).normalized;
    public void Dodge()
    {
        Debug.Log(mousePos);
        GameObject.Find("SUCE").transform.position = mousePos;
        animator.Dodge();
        Vector3 dir = mouseDir;
        p.RegisterPropulsion(dir, m.dodge);
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
