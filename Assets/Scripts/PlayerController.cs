using System;
using DG.Tweening;
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

    public Transform PlayerModel;
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

    private Transform _currentMoveableParent;
    private bool _secondJumpAvailable;
    private bool _airborneDashAvailable;
    private bool _reversing;
    private bool _startedDying;


    private Rigidbody _rigidbody;

    public bool DoubleJumpUnlocked;
    public bool DashUnlocked;

    private TemporalManager _temporalManager;

    public bool IsDead;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.detectCollisions = false;
        _animationController = GetComponent<PlayerAnimationController>();
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = FindObjectOfType<CameraControl>();
        _temporalManager = FindObjectOfType<TemporalManager>();
        _jumpParticles = GetComponentInChildren<ParticleSystem>();
        _dashTrailRenderer = GetComponent<TrailRenderer>();
        _dashTrailRenderer.enabled = false;
        InputActionsManager.InputActions.Player.Move.performed += ctx => MoveDirectionInput = -ctx.ReadValue<Vector2>();
        InputActionsManager.InputActions.Player.Move.canceled += _ => MoveDirectionInput = Vector2.zero;

        InputActionsManager.InputActions.Player.Jump.started += _ => TryJump();
        InputActionsManager.InputActions.Player.ResetLevel.started += _ => TryResetLevel();
        InputActionsManager.InputActions.Player.Dash.started += _ => TryDash();


        LevelLoaderManager.Instance.StartedLevelExit += OnStartedLevelExit;
    }

    private bool _resetLevelQueued;
    private void TryResetLevel()
    {
        Time.timeScale = 1f;
        LevelLoaderManager.RestartCurrentLevel();
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
        if (_inputDisabled || _reversing || !LevelLoaderManager.Instance.IsLevelLoaded || _temporalManager.LevelEndReached() || IsDead || _startedDying)
            return;
        
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        RefreshMovementDirection();

        Move(Time.deltaTime);

        var newGroundedState = _characterController.isGrounded ? GroundedState.GROUNDED : GroundedState.AIRBORNE;
        SetGroundedState(newGroundedState);

        var moveableMask = LayerMask.GetMask("Moveable");
        var moveableRay = new Ray(transform.position, Vector3.down);
        var hitMoveable = Physics.Raycast(moveableRay, out var hitInfo, 1f, moveableMask);

        if (hitMoveable)
        {
            var moveable = hitInfo.transform;
            transform.parent = moveable;
            _currentMoveableParent = moveable;
        }
        else
        {
            _currentMoveableParent = null;
            transform.parent = null;
        }
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
                VerticalSpeed = -0.1f;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * timeStep);
    }

    private void TryDash()
    {
        if (IsDead || !DashUnlocked)
        {
            return;
        }

        if (IsDashing || !_airborneDashAvailable ||
             !(Time.time > _lastDashTime + PlayerMovementSettings.DashCooldown))
        {
            return;
        }

        IsDashing = true;
        _dashTrailRenderer.enabled = true;
        _animationController.Dash();
        _dashDirection = new Vector3(_facingDirection.x, 0, _facingDirection.y);
        _lastDashTime = Time.time;
        AudioManager.Play("dash");
        if (GroundedState == GroundedState.AIRBORNE)
        {
            _airborneDashAvailable = false;
        }
    }

    private void TryJump()
    {
        if (IsDead)
        {
            return;
        }
        
        if (GroundedState == GroundedState.GROUNDED && !IsDashing && Time.time > _lastJumpTime + _jumpCooldown)
        {
            _lastJumpTime = Time.time;
            _jumpCooldown = PlayerMovementSettings.JumpCooldownGround;
            VerticalSpeed = PlayerMovementSettings.JumpSpeed;
            GroundedState = GroundedState.AIRBORNE;

            _secondJumpAvailable = true;
            _animationController.Jump();
            _jumpParticles.Play();
            AudioManager.Play("jump");
        }
        else if (GroundedState == GroundedState.AIRBORNE && _secondJumpAvailable && DoubleJumpUnlocked)
        {
            VerticalSpeed = PlayerMovementSettings.JumpSpeed;
            _secondJumpAvailable = false;
            _animationController.DoubleJump();
            _jumpParticles.Play();
            AudioManager.Play("jump");
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

    private void InteractWithRigidbody(Rigidbody otherRigidbody)
    {
        if (otherRigidbody == null)
        {
            return;
        }

        var temporal = otherRigidbody.GetComponent<RigidbodySpatialTemporal>();

        if (temporal == null)
            return;

        temporal.OnInteractedWith();
    }

    // Used by temporal manager to disable input when reversing level
    public void DisableInputAndAnimations()
    {
        _inputDisabled = true;
        _reversing = true;
        _currentMoveableParent = null;
        transform.parent = null;
        _animationController.PauseAnimations();
    }

    public void EnableInputAndAnimations()
    {
        _inputDisabled = false;
        _reversing = false;
        _animationController.ResumeAnimations();
    }

    public void OnResetTemporal()
    {
        EnableInputAndAnimations();
        _targetLookDirection = _startingTargetLookDirection;
    }
    
    public void Die()
    {
        if (IsDead || _startedDying)
        {
            return;
        }
        _startedDying = true;

        _dashTrailRenderer.enabled = false;
        _animationController.PauseAnimations();
        PlayerModel.DOScale(Vector3.zero, 1f)
            .SetEase(Ease.OutCirc)
            .OnComplete(() =>
            {
                // HACK HACK HACK
                var uiManager = FindObjectOfType<GameUIManager>();
                uiManager.ShowReminderText(true);
                IsDead = true;
                _startedDying = false;
            });
    }
    
    private void OnStartedLevelExit(object sender, EventArgs e)
    {
        DisableInputAndAnimations();
    }

    private void OnDestroy()
    {
        LevelLoaderManager.Instance.StartedLevelExit -= OnStartedLevelExit;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
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
    }
}