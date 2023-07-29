using UnityEngine;
using UnityEngine.Rendering;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using Random = UnityEngine.Random;

public class CameraControl : MonoBehaviour
{
    public Volume PostProcessingVolume;
    public PlayerController PlayerController;
    private Vector3 _offset;

    private float _currentShakeDuration;
    private float _currentShakeAmount;
    private Vector3 _shakeOffset;
    private bool _cameraDragButtonDown;
    public float ShakeDecreaseFactor;
    private Vector2 _mousePosition;
    private Vector2 _startMousePos;
    private bool _dragging;
    
    private void Start()
    {
        _offset = PlayerController.transform.position - transform.position;
        
        InputActionsManager.InputActions.Camera.MousePosition.performed += ctx => _mousePosition = ctx.ReadValue<Vector2>();
        InputActionsManager.InputActions.Camera.MousePosition.canceled += _ => _mousePosition = Vector2.zero;

        InputActionsManager.InputActions.Camera.CameraDragButton.performed += _ => _cameraDragButtonDown = true;
        InputActionsManager.InputActions.Camera.CameraDragButton.canceled += _ => _cameraDragButtonDown = false;
        
        var targetLookRotation = Quaternion.LookRotation(PlayerController.transform.position - transform.position, Vector3.up);
        transform.rotation = targetLookRotation;
    }

    public void Shake(float amount, float duration)
    {
        _currentShakeAmount = amount;
        _currentShakeDuration = duration;
    }

    private void Update()
    {
        if (!_cameraDragButtonDown)
        {
            _startMousePos = Vector2.zero;
            _dragging = false;
            UpdateCameraPosition();
        }
        else if (_cameraDragButtonDown)
        {
            if (_dragging)
            {
                var horizontalDelta = (_mousePosition - _startMousePos).x;
        
                var horizontalMovementRatio = horizontalDelta / Screen.width;
                
                var rotationQuaternion = Quaternion.AngleAxis(horizontalMovementRatio * 15, Vector3.up);
                _offset = rotationQuaternion * _offset;
        
                UpdateCameraPosition();
                
                var targetLookRotation = Quaternion.LookRotation(PlayerController.transform.position - transform.position, Vector3.up);
                transform.rotation = targetLookRotation;
            }
            else
            {
                UpdateCameraPosition();
                _startMousePos = _mousePosition;
                _dragging = true;
            }
        }


        if (_currentShakeDuration > 0)  
        {
            _shakeOffset = Random.insideUnitSphere * _currentShakeAmount;
            _currentShakeDuration -= Time.deltaTime * ShakeDecreaseFactor;
        }
    }

    private void UpdateCameraPosition()
    {
        var targetPosition = PlayerController.transform.position - _offset + _shakeOffset;
        transform.position = targetPosition;
    }
}