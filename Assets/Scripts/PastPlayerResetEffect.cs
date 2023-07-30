using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class PastPlayerResetEffect : MonoBehaviour
    {
        [SerializeField]
        private float _duration = 0.6f;
        
        [SerializeField]
        private Vector3 _portalMaxScale = new Vector3(2f, 3.2f, 2f);
        
        [SerializeField]
        private Transform _portal;

        [SerializeField]
        private Transform _playerModel;
        
        private Tween _portalTween;
        private Tween _playerModelTween;

        private void Start()
        {
            RotatePortalToPointUp();
            _portal.localScale = Vector3.zero;
            _portalTween = _portal.DOScale(_portalMaxScale, _duration * 1 / 3)
                .SetEase(Ease.InOutBack)
                .OnComplete(ShrinkPortalAndPlayer);
        }

        private void RotatePortalToPointUp()
        {
            var worldUpDirection = Vector3.up;
            var targetRotation = Quaternion.FromToRotation(transform.up, worldUpDirection) * transform.rotation;

            _portal.rotation = targetRotation;
        }
        

        private void ShrinkPortalAndPlayer()
        {
            _portalTween = _portal.DOScale(Vector3.zero, _duration * 2 / 3)
                .SetEase(Ease.InOutBack); 

            _playerModelTween = _playerModel.DOScale(Vector3.zero, _duration * 2 / 3)
                .SetEase(Ease.InOutBack)
                .OnComplete(OnEffectComplete);
        }

        private void OnEffectComplete()
        {
            _portalTween.Kill();
            _playerModelTween.Kill();
            
            Destroy(gameObject);
        }
    }
}