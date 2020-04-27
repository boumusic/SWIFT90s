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
        stateMachine.ChangeState(CharacterState.Grounded);
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
    }

    private void OnDrawGizmos()
    {
        DrawBox();
    }

    #endregion

    #region Input

    private void DebugInput()
    {
        InputHorizontal(Input.GetAxisRaw("Horizontal"));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartJump();
        }
    }

    public void InputHorizontal(float horizontal)
    {
        this.horizontalAxis = Mathf.Abs(horizontal) < 0.2f ? 0 : Mathf.Sign(horizontal);
    }

    #endregion

    #region Movement

    private void CalculateHorizontalAcceleration()
    {
        if (horizontalAxis != 0)
        {
            xAccel += horizontalAxis * m.groundedAcceleration * Time.deltaTime;
        }

        else
        {
            xAccel /= (m.groundedDeceleration);
        }

        xAccel = Mathf.Clamp(xAccel, -1f, 1f);
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
        body.velocity = velocity;
    }

    #endregion

    #region Grounded

    private void Grounded_Enter()
    {
        SetVerticalVelocity(0);
        ResetJumpCount();
    }

    private void Grounded_Update()
    {
        if(!CastGround())
        {
            stateMachine.ChangeState(CharacterState.Falling);
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
        jumpProgress = 0f;
    }

    private void Jumping_Update()
    {
        jumpProgress += Time.deltaTime / m.jumpDuration;
        float strength = m.jumpCurve.Evaluate(jumpProgress) * m.jumpStrength;
        SetVerticalVelocity(strength);
        if (jumpProgress > 1f)
        {
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
        fallProgress += Time.deltaTime / m.timeToReachMaxFall;
        fallProgress = Mathf.Clamp01(fallProgress);
        float fall = Mathf.Lerp(yVelocityStartFall, -m.maxFallSpeed, m.fallCurve.Evaluate(fallProgress));
        SetVerticalVelocity(fall);
        if(CastGround())
        {
            stateMachine.ChangeState(CharacterState.Grounded);
        }
    }

    #endregion

    #region Cast

    public Vector3 FeetOrigin => transform.position + Vector3.up * m.groundRaycastUp;
    public Vector3 CastBox => new Vector3(m.castBoxWidth, 0, m.castBoxWidth);
    private RaycastHit hit;

    private void DrawBox()
    {
        BoxCastDrawer.DrawBoxCastBox(FeetOrigin, CastBox * m.groundCastRadius, Quaternion.identity, -Vector3.up, m.groundRaycastDown, Color.red);
    }

    public bool CastGround()
    {
        if (Physics.BoxCast(FeetOrigin, CastBox * m.groundCastRadius, -Vector3.up, out hit, Quaternion.identity, m.groundRaycastDown, m.groundMask))
        {
            SnapToGround();
            return true;
        }
        return false;
    }

    private void SnapToGround()
    {
        body.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
    }

    #endregion
}

public enum CharacterState
{
    Grounded,
    Jumping,
    Falling,
    WallClimbing
}
