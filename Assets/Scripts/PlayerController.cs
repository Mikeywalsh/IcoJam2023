using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementSettings PlayerMovementSettings;
    public Vector2 MoveDirection;
    public Vector2 MoveDirectionInput;
    public bool JumpInput;
    public bool RollInput;

    public float TargetHorizontalSpeed; // In meters/second
    public float HorizontalSpeed; // In meters/second
    public float VerticalSpeed; // In meters/second
    private Vector3 _rollDirection;

    public bool IsDashing { get; private set; }
    public GroundedState GroundedState { get; private set; }
    
    private bool _itemSelectionBlocked;
    private Action _planeTransitionCallback;
    private CharacterController _characterController;
    private Vector3 _facingDirection;
    private Vector3 _playerVelocity;
    private float _lastJumpTime = -100f;
    private float _lastRollTime = -100f;
    private float _lastGroundedTime = -100f;
    private float _jumpCooldown;
    private float _useHeldItemCooldown;
    private float _verticalAcceleration;
    private Vector3 _targetLookDirection = Vector3.back;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        InputActionsManager.InputActions.Player.Move.performed += ctx => MoveDirectionInput = -ctx.ReadValue<Vector2>();
        InputActionsManager.InputActions.Player.Move.canceled += _ => MoveDirectionInput = Vector2.zero;

        InputActionsManager.InputActions.Player.Jump.performed += _ => JumpInput = true;
        InputActionsManager.InputActions.Player.Jump.canceled += _ => JumpInput = false;


        // InputActionsManager.InputActions.Player.Roll.performed += _ => RollInput = true;
        // InputActionsManager.InputActions.Player.Roll.canceled += _ => RollInput = false;

        // InputActionsManager.InputActions.Player.Interact.performed += _ => InteractInput = true;
        // InputActionsManager.InputActions.Player.Interact.canceled += _ => InteractInput = false;
    }

    private void RefreshMovementDirection()
    {
        if (MoveDirectionInput.magnitude < float.Epsilon)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        var cameraVector = transform.position - Camera.main.transform.position;
        var cross = Vector3.Cross(cameraVector, Vector3.up);

        var playerRight = cross.normalized;
        var playerForward = Vector3.Cross(cross, Vector3.up).normalized;

        var movement = Vector3.zero;

        movement += playerForward * MoveDirectionInput.y;
        movement += playerRight * MoveDirectionInput.x;

        MoveDirection = new Vector2(movement.normalized.x, movement.normalized.z);
    }

    private void FixedUpdate()
    {
        RefreshMovementDirection();

        Move(Time.deltaTime);

        var newGroundedState = _characterController.isGrounded ? GroundedState.GROUNDED : GroundedState.AIRBORNE;
        SetGroundedState(newGroundedState);
    }

    private void UpdateHorizontalSpeed(float timeStep)
    {
        float acceleration;

        // if (RollInput)
        // {
        //     TryDash();
        // }

        // if (IsDashing)
        // {
        //     var rollDuration = Time.time - _lastRollTime;
        //     acceleration = PlayerMovementSettings.DashSpeed;
        //
        //     if (rollDuration <= PlayerMovementSettings.TotalDashDuration)
        //     {
        //         var rollProgressPercent = rollDuration / PlayerMovementSettings.TotalDashDuration;
        //
        //         var rollSpeedReductionFactor = Mathf.Cos(Mathf.Lerp(0, Mathf.PI / 2, rollProgressPercent * 0.75f));
        //
        //         // Slow roll down more towards end of roll
        //         if (rollProgressPercent > 0.65f)
        //         {
        //             rollSpeedReductionFactor /= 1.5f;
        //         }
        //
        //         var rollSpeed = _isRollFromSprint
        //             ? PlayerMovementSettings.RollSpeedFromSprint
        //             : PlayerMovementSettings.DashSpeed;
        //         TargetHorizontalSpeed = rollSpeed * rollSpeedReductionFactor;
        //     }
        //     else
        //     {
        //         IsDashing = false;
        //     }
        // }
        // else
        // {
        if (GroundedState == GroundedState.GROUNDED)
        {
            TargetHorizontalSpeed = MoveDirection.magnitude * PlayerMovementSettings.MaxHorizontalRunSpeed;
        }
        else if (GroundedState == GroundedState.AIRBORNE)
        {
            TargetHorizontalSpeed = MoveDirection.magnitude * PlayerMovementSettings.MaxHorizontalRunSpeed;
        }

        var hasMovementInput = MoveDirection.sqrMagnitude > float.Epsilon;

        acceleration = hasMovementInput
            ? PlayerMovementSettings.HorizontalAcceleration
            : PlayerMovementSettings.HorizontalDeceleration;


        HorizontalSpeed = Mathf.MoveTowards(HorizontalSpeed, TargetHorizontalSpeed, acceleration * timeStep);
    }

    private void UpdateVerticalSpeed(float timeStep)
    {
        var accelerationMultiplier =
            GroundedState == GroundedState.AIRBORNE && VerticalSpeed < -float.Epsilon ? 1.5f : 1f;
        _verticalAcceleration = -PlayerMovementSettings.GravityStrength * accelerationMultiplier;
        
        switch (GroundedState)
        {
            case GroundedState.GROUNDED
                : // If grounded, set speed to essentially 0 (still needs to be higher or the player floats and causes weird collider issues)
                VerticalSpeed = -PlayerMovementSettings.GravityStrength;
                break;
            case GroundedState.AIRBORNE:
                VerticalSpeed = Mathf.Clamp(VerticalSpeed + _verticalAcceleration * timeStep,
                    -PlayerMovementSettings.MaxFallSpeedAir,
                    PlayerMovementSettings.MaxFallSpeedAir);
                break;
        }
        
        if (JumpInput)
        {
            TryJump();
        }
    }

    public void Move(float timeStep)
    {
        UpdateHorizontalSpeed(timeStep);
        UpdateVerticalSpeed(timeStep);

        var horizontalMovementVector = IsDashing ? _rollDirection : new Vector3(MoveDirection.x, 0, MoveDirection.y);
        var localMovement = HorizontalSpeed * horizontalMovementVector + VerticalSpeed * Vector3.up;
        var movement = localMovement;

        _characterController.Move(movement);

        if (horizontalMovementVector.magnitude > float.Epsilon)
        {
            _targetLookDirection = horizontalMovementVector;
        }
        
        var targetRotation = Quaternion.LookRotation(_targetLookDirection, Vector3.up);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 15 * timeStep);
    }

    // private void TryDash()
    // {
    //     if (GroundedState == GroundedState.GROUNDED &&
    //         !IsDashing &&
    //         Time.time > _lastFullBodyAnimationExitTime + POST_FULL_BODY_ANIMATION_ROLL_DELAY &&
    //         Time.time > _lastRollTime + PlayerMovementSettings.DashCooldown)
    //     {
    //         OnRollPerformed(new RollPerformedComboEventArgs {FromCombo = false});
    //     }
    // }

    private void TryJump()
    {
        if (GroundedState != GroundedState.AIRBORNE && !IsDashing && Time.time > _lastJumpTime + _jumpCooldown)
        {
            _lastJumpTime = Time.time;
            VerticalSpeed = PlayerMovementSettings.JumpSpeed;

            if (GroundedState == GroundedState.GROUNDED)
            {
                _jumpCooldown = PlayerMovementSettings.JumpCooldownGround;
            }

            GroundedState = GroundedState.AIRBORNE;
        }
    }

    private void SetGroundedState(GroundedState newState)
    {
        if (newState == GroundedState.GROUNDED && GroundedState != GroundedState.GROUNDED)
        {
            // Don't allow player to use held items immediately after landing
            _lastGroundedTime = Time.time;
            _jumpCooldown = PlayerMovementSettings.JumpCooldownGround;
        }

        if (newState == GroundedState.AIRBORNE && GroundedState != GroundedState.AIRBORNE)
        {
            if (VerticalSpeed < 0.1f)
            {
                VerticalSpeed = -PlayerMovementSettings.GravityStrength * 0.1f;
            }
        }

        GroundedState = newState;
    }


    // protected virtual void OnDashPerformed(RollPerformedComboEventArgs args)
    // {
    //     _lastRollTime = Time.time;
    //     _rollDirection = MoveDirection.magnitude < float.Epsilon
    //         ? transform.InverseTransformVector(PlayerModel.forward.normalized)
    //         : transform.InverseTransformVector(new Vector3(MoveDirection.x, 0, MoveDirection.y));
    //     IsDashing = true;
    //
    //     _isRollFromSprint = HorizontalSpeed > PlayerMovementSettings.MaxHorizontalRunSpeed;
    //
    //     RollPerformed?.Invoke(this, args);
    // }

    private void OnCollisionEnter(Collision other)
    {
        InteractWithRigidbody(other.rigidbody);
    }

    private void OnCollisionStay(Collision other)
    {
        InteractWithRigidbody(other.rigidbody);
    }

    private void InteractWithRigidbody(Rigidbody otherRigidbody)
    {
        if (otherRigidbody == null)
        {
            return;
        }

        var temporal = otherRigidbody.GetComponent<RigidbodySpatialTemporal>();
        temporal.OnInteractedWith();
        
        var forceVector = (otherRigidbody.transform.position - transform.position).normalized;
        var pushForce = 4;
        otherRigidbody.AddForce(forceVector * pushForce, ForceMode.Force);
    }

    public void OnDrawGizmos()
    {
        var cameraVector = transform.position - Camera.main.transform.position;
        var playerLeft = Vector3.Cross(cameraVector, Vector3.up).normalized;
        var playerBack = Vector3.Cross(playerLeft, Vector3.up).normalized;

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(Camera.main.transform.position, cameraVector);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, playerLeft);
        Gizmos.DrawRay(transform.position, -playerLeft);
        Gizmos.DrawRay(transform.position, playerBack);
        Gizmos.DrawRay(transform.position, -playerBack);
    }
}