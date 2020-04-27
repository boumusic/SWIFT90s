﻿using System.Collections;
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
        body.velocity = velocity;
    }

    #endregion

    #region Grounded

    private bool grounded => CurrentState == CharacterState.Grounded;

    private void Grounded_Enter()
    {
        Debug.Log("Enter Ground");
        SetVerticalVelocity(0);
        ResetJumpCount();
        animator.Land();
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
        animator.Jump(jumpLeft == 0);
        jumpProgress = 0f;
        SnapAccelToAxis();
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
            SnapToGround();
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
            return true;
        }
        return false;
    }

    private void SnapToGround()
    {
        body.position = new Vector3(body.position.x, hit.point.y, body.position.z);
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
    WallClimbing
}
