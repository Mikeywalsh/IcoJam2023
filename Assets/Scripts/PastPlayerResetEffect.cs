using System;
using DG.Tweening;
using UnityEngine;

public class PastPlayerResetEffect : MonoBehaviour
{
    [SerializeField] private float _duration = 0.6f;

    [SerializeField] private Vector3 _portalMaxScale = new Vector3(2f, 3.2f, 2f);

    [SerializeField] private Transform _portal;

    [SerializeField] private Transform _playerModel;

    private Tween _portalTween;
    private Tween _playerModelTween;

    private CameraControl _mainCamera;

    private void Start()
    {
        _mainCamera = FindObjectOfType<CameraControl>();
        _portal.localScale = Vector3.zero;
        _portalTween = _portal.DOScale(_portalMaxScale, _duration / 3)
            .SetEase(Ease.InOutBack)
            .OnComplete(ShrinkPortalAndPlayer);
    }

    private void ShrinkPortalAndPlayer()
    {
        _portalTween = _portal.DOScale(Vector3.zero, _duration * 2 / 3)
            .SetEase(Ease.InOutBack);

        _playerModelTween = _playerModel.DOScale(Vector3.zero, _duration * 2 / 3)
            .SetEase(Ease.InOutBack)
            .OnComplete(OnEffectComplete);
    }

    private void Update()
    {
        // Always show portal effect over player model
        _portal.transform.position =
            transform.position + (_mainCamera.transform.position - transform.position).normalized * 1f;
    }

    private void OnEffectComplete()
    {
        _portalTween.Kill();
        _playerModelTween.Kill();

        Destroy(gameObject);
    }
}