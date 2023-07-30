using System;
using UnityEngine;


[RequireComponent(typeof(PlayerAnimationController))]
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
    private Vector3 _dashDirection;

    public bool IsDashing { get; private set; }
    public GroundedState GroundedState { get; private set; }

    private PlayerAnimationController _animationController;
    private ParticleSystem _jumpParticles;
    private TrailRenderer _dashTrailRenderer;
    
    private bool _itemSelectionBlocked;
    private Action _planeTransitionCallback;
    private CharacterController _characterController;
    private CameraControl _mainCamera;
    private Vector3 _facingDirection;
    private Vector3 _playerVelocity;
    private float _lastJumpTime = -100f;
    private float _lastDashTime = -100f;
    private float _jumpCooldown;
    private float _useHeldItemCooldown;
    private float _verticalAcceleration;
    private Vector3 _targetLookDirection = Vector3.back;
    private Vector3 _startingTargetLookDirection = Vector3.back;
    private bool _inputDisabled;
    
    private bool _secondJumpAvailable;
    private bool _airborneDashAvailable;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.detectCollisions = false;
        _animationController = GetComponent<PlayerAnimationController>();
        _mainCamera = FindObjectOfType<CameraControl>();
        _jumpParticles = GetComponentInChildren<ParticleSystem>();
        _dashTrailRenderer = GetComponent<TrailRenderer>();
        _dashTrailRenderer.enabled = false;
        InputActionsManager.InputActions.Player.Move.performed += ctx => MoveDirectionInput = -ctx.ReadValue<Vector2>();
        InputActionsManager.InputActions.Player.Move.canceled += _ => MoveDirectionInput = Vector2.zero;

        InputActionsManager.InputActions.Player.Jump.started += _ => TryJump();
        InputActionsManager.InputActions.Player.Dash.started += _ => TryDash();
    }

    private void RefreshMovementDirection()
    {
        if (MoveDirectionInput.magnitude < float.Epsilon)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        var cameraVector = transform.position - _mainCamera.transform.position;
        var cross = Vector3.Cross(cameraVector, Vector3.up);

        var playerRight = cross.normalized;
        var playerForward = Vector3.Cross(cross, Vector3.up).normalized;

        var movement = Vector3.zero;

        movement += playerForward * MoveDirectionInput.y;
        movement += playerRight * MoveDirectionInput.x;

        MoveDirection = new Vector2(movement.normalized.x, movement.normalized.z);
        _facingDirection = MoveDirection;
    }

    private void Update()
    {
        var isRunning = HorizontalSpeed > float.Epsilon;
        _animationController.SetIsRunning(isRunning);
    }
    
    private void FixedUpdate()
    {
        if (_inputDisabled)
            return;
        
        RefreshMovementDirection();

        Move(Time.deltaTime);

        var newGroundedState = _characterController.isGrounded ? GroundedState.GROUNDED : GroundedState.AIRBORNE;
        SetGroundedState(newGroundedState);
    }

    private void UpdateHorizontalSpeed(float timeStep)
    {
        if (IsDashing)
        {
            var dashDuration = Time.time - _lastDashTime;
            TargetHorizontalSpeed = PlayerMovementSettings.HorizontalDashSpeed;

            HorizontalSpeed = TargetHorizontalSpeed;
            if (dashDuration > PlayerMovementSettings.TotalDashDuration)
            {
                HorizontalSpeed = 0f;
                TargetHorizontalSpeed = 0f;
                IsDashing = false;
            }
        } 
        
        if (!IsDashing)
        {
            _dashTrailRenderer.enabled = false;
            TargetHorizontalSpeed = MoveDirection.magnitude * PlayerMovementSettings.MaxHorizontalRunSpeed;
            var hasMovementInput = MoveDirection.sqrMagnitude > float.Epsilon;

            var acceleration = hasMovementInput
                ? PlayerMovementSettings.HorizontalAcceleration
                : PlayerMovementSettings.HorizontalDeceleration;
            
            HorizontalSpeed = Mathf.MoveTowards(HorizontalSpeed, TargetHorizontalSpeed, acceleration * timeStep);
        }
        
    }

    private void UpdateVerticalSpeed(float timeStep)
    {
        if (IsDashing)
        {
            VerticalSpeed = 0f;
        }
        
        var accelerationMultiplier =
            GroundedState == GroundedState.AIRBORNE && VerticalSpeed < -float.Epsilon ? 1.5f : 1f;
        _verticalAcceleration = -PlayerMovementSettings.GravityStrength * accelerationMultiplier;

        switch (GroundedState)
        {
            case GroundedState.GROUNDED: 
                 // If grounded, set speed to essentially 0 (still needs to be higher or the player floats and causes weird collider issues)
                VerticalSpeed = -PlayerMovementSettings.GravityStrength;
                _secondJumpAvailable = false;
                _airborneDashAvailable = true;
                break;
            case GroundedState.AIRBORNE:
                VerticalSpeed = Mathf.Clamp(VerticalSpeed + _verticalAcceleration * timeStep,
                    -PlayerMovementSettings.MaxFallSpeedAir,
                    PlayerMovementSettings.MaxFallSpeedAir);
                break;
        }
    }

    public void Move(float timeStep)
    {
        UpdateHorizontalSpeed(timeStep);
        UpdateVerticalSpeed(timeStep);

        var horizontalMovementVector = IsDashing ? _dashDirection : new Vector3(MoveDirection.x, 0, MoveDirection.y);
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

    private void TryDash()
    {
        if (IsDashing || !_airborneDashAvailable || !(Time.time > _lastDashTime + PlayerMovementSettings.DashCooldown))
        {
            return;
        }

        IsDashing = true;
        _dashTrailRenderer.enabled = true;
        _animationController.Dash();
        _dashDirection = new Vector3(_facingDirection.x, 0, _facingDirection.y);
        _lastDashTime = Time.time;
        if (GroundedState == GroundedState.AIRBORNE)
        {
            _airborneDashAvailable = false;
        }
    }

    private void TryJump()
    {
        if (GroundedState == GroundedState.GROUNDED && !IsDashing && Time.time > _lastJumpTime + _jumpCooldown)
        {
            _lastJumpTime = Time.time;
            _jumpCooldown = PlayerMovementSettings.JumpCooldownGround;
            VerticalSpeed = PlayerMovementSettings.JumpSpeed;
            GroundedState = GroundedState.AIRBORNE;

            _secondJumpAvailable = true;
            _animationController.Jump();
            _jumpParticles.Play();
        } 
        else if (GroundedState == GroundedState.AIRBORNE && _secondJumpAvailable)
        {
            VerticalSpeed = PlayerMovementSettings.JumpSpeed;
            _secondJumpAvailable = false;
            _animationController.DoubleJump();
            _jumpParticles.Play();
        }
    }

    private void SetGroundedState(GroundedState newState)
    {
        if (newState == GroundedState.GROUNDED && GroundedState != GroundedState.GROUNDED)
        {
            _jumpCooldown = PlayerMovementSettings.JumpCooldownGround;
            _jumpParticles.Play();
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

    private void OnCollisionEnter(Collision other)
    {
        InteractWithRigidbody(other.rigidbody);
    }

    private void OnCollisionStay(Collision other)
    {
        InteractWithRigidbody(other.rigidbody);
    }

    private Vector3 fooOrigin;
    private Vector3 fooDirection;
    
    private void InteractWithRigidbody(Rigidbody otherRigidbody)
    {
        if (otherRigidbody == null)
        {
            return;
        }

        var temporal = otherRigidbody.GetComponent<RigidbodySpatialTemporal>();
        Debug.Log("CHEESE");

        temporal.OnInteractedWith();
        //
        // var forceVector = (otherRigidbody.transform.position - transform.position).normalized;
        //
        // var layerMask = LayerMask.GetMask("Environment");
        // var boxWillHitEnvironment = Physics.Raycast(otherRigidbody.transform.position + (forceVector * 1.1f), forceVector, 2.2f, layerMask);
        //
        // fooOrigin = otherRigidbody.transform.position + (forceVector * 1.1f);
        // fooDirection = forceVector *.5f;
        // Debug.Log(boxWillHitEnvironment);
        // if(boxWillHitEnvironment)
        //     return;
        
        // var pushForce = 4;
        // otherRigidbody.AddForce(forceVector * pushForce, ForceMode.Force);
        
    }

    // Used by temporal manager to disable input when reversing level
    public void DisableInputAndAnimations()
    {
        _inputDisabled = true;
        _animationController.PauseAnimations();
    }

    public void EnableInputAndAnimations()
    {
        _inputDisabled = false;
        _animationController.ResumeAnimations();
    }

    public void OnResetTemporal()
    {
        EnableInputAndAnimations();
        _targetLookDirection = _startingTargetLookDirection;
    }
    
    public void Die()
    {
        Debug.Log("player died");
    }

    public void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;
        
        var cameraPosition = _mainCamera.transform.position;
        var position = transform.position;
        var cameraVector = position - cameraPosition;
        var playerLeft = Vector3.Cross(cameraVector, Vector3.up).normalized;
        var playerBack = Vector3.Cross(playerLeft, Vector3.up).normalized;

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(cameraPosition, cameraVector);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(position, playerLeft);
        Gizmos.DrawRay(position, -playerLeft);
        Gizmos.DrawRay(position, playerBack);
        Gizmos.DrawRay(position, -playerBack);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(fooOrigin, fooDirection);
    }
}