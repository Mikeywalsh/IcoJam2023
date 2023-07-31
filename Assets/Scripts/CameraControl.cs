using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using Random = UnityEngine.Random;

public class CameraControl : MonoBehaviour
{
    public Volume PostProcessingVolume;

    public Color ColorFilterColor;

    private LensDistortion _lensDistortionComponent;
    private ColorAdjustments _ColorAdjustmentComponent;
    private FilmGrain _filmGrainComponent;

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

    private float _cameraDragGamepadValue;
    
    private void Start()
    {
        if (PostProcessingVolume.profile.TryGet<LensDistortion>(out var lensDistortion))
        {
            _lensDistortionComponent = lensDistortion;
        }
        else
        {
            throw new Exception("No LensDistortion component found...");
        }

        if (PostProcessingVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            _ColorAdjustmentComponent = colorAdjustments;
        }
        else
        {
            throw new Exception("No ColorAdjustments component found...");
        }

        if (PostProcessingVolume.profile.TryGet<FilmGrain>(out var filmGrain))
        {
            _filmGrainComponent = filmGrain;
        }
        else
        {
            throw new Exception("No FilmGrain component found...");
        }

        _offset = PlayerController.transform.position - transform.position;

        InputActionsManager.InputActions.Camera.MousePosition.performed +=
            ctx => _mousePosition = ctx.ReadValue<Vector2>();
        InputActionsManager.InputActions.Camera.MousePosition.canceled += _ => _mousePosition = Vector2.zero;

        InputActionsManager.InputActions.Camera.CameraDragButton.performed += _ => _cameraDragButtonDown = true;
        InputActionsManager.InputActions.Camera.CameraDragButton.canceled += _ => _cameraDragButtonDown = false;
        
        InputActionsManager.InputActions.Camera.CameraDragDirection.performed += ctx => _cameraDragGamepadValue = ctx.ReadValue<Vector2>().x;
        InputActionsManager.InputActions.Camera.CameraDragDirection.canceled += _ => _cameraDragGamepadValue = 0;

        var targetLookRotation =
            Quaternion.LookRotation(PlayerController.transform.position - transform.position, Vector3.up);
        transform.rotation = targetLookRotation;
    }

    public void Shake(float amount, float duration)
    {
        _currentShakeAmount = amount;
        _currentShakeDuration = duration;
    }

    private void Update()
    {
        if (InputActionsManager.CurrentInputScheme == InputScheme.MOUSE_KEYBOARD)
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

                    var targetLookRotation =
                        Quaternion.LookRotation(PlayerController.transform.position - transform.position, Vector3.up);
                    transform.rotation = targetLookRotation;
                }
                else
                {
                    UpdateCameraPosition();
                    _startMousePos = _mousePosition;
                    _dragging = true;
                }
            }

        }
        else if (InputActionsManager.CurrentInputScheme == InputScheme.CONTROLLER)
        {
            var horizontalDelta = _cameraDragGamepadValue;

            var rotationQuaternion = Quaternion.AngleAxis(horizontalDelta * 2.5f, Vector3.up);
            _offset = rotationQuaternion * _offset;

            UpdateCameraPosition();

            var targetLookRotation =
                Quaternion.LookRotation(PlayerController.transform.position - transform.position, Vector3.up);
            transform.rotation = targetLookRotation;
        }

        if (_currentShakeDuration > 0)
        {
            _shakeOffset = Random.insideUnitSphere * _currentShakeAmount;
            _currentShakeDuration -= Time.deltaTime * ShakeDecreaseFactor;
        }
    }

    public void OnLevelReverse(float reverseDuration)
    {
        var distortionProgress = 0f;
        DOTween.To(() => distortionProgress, x => distortionProgress = x, -.8f, .4f)
            .SetEase(Ease.OutQuart)
            .OnUpdate(() => { _lensDistortionComponent.intensity.Override(distortionProgress); }).OnComplete(() =>
            {
                DOTween.To(() => distortionProgress, x => distortionProgress = x, 0f, reverseDuration)
                    .SetEase(Ease.InCirc)
                    .OnUpdate(() => { _lensDistortionComponent.intensity.Override(distortionProgress); });
            });

        var filmGrainProgress = 0f;
        DOTween.To(() => filmGrainProgress, x => filmGrainProgress = x, 1f, reverseDuration / 2)
            .SetEase(Ease.OutQuart)
            .OnUpdate(() => { _filmGrainComponent.intensity.Override(filmGrainProgress); }).OnComplete(() =>
            {
                DOTween.To(() => distortionProgress, x => filmGrainProgress = x, 0f, reverseDuration / 2)
                    .SetEase(Ease.InCirc)
                    .OnUpdate(() => { _filmGrainComponent.intensity.Override(filmGrainProgress); });
            });

        var colorAdjustmentProgress = 0f;
        DOTween.To(() => colorAdjustmentProgress, x => colorAdjustmentProgress = x, 1f, reverseDuration / 2)
            .SetEase(Ease.OutQuart)
            .OnUpdate(() =>
            {
                var currentColorFilterColor = Color.Lerp(Color.white, ColorFilterColor, colorAdjustmentProgress);
                _ColorAdjustmentComponent.colorFilter.Override(currentColorFilterColor);
            }).OnComplete(() =>
            {
                DOTween.To(() => colorAdjustmentProgress, x => colorAdjustmentProgress = x, 0f, reverseDuration / 2)
                    .SetEase(Ease.InCirc)
                    .OnUpdate(() =>
                    {
                        var currentColorFilterColor = Color.Lerp(Color.white, ColorFilterColor, colorAdjustmentProgress);
                        _ColorAdjustmentComponent.colorFilter.Override(currentColorFilterColor);
                    });
            });
    }

    private void UpdateCameraPosition()
    {
        var targetPosition = PlayerController.transform.position - _offset + _shakeOffset;
        transform.position = targetPosition;
    }
}