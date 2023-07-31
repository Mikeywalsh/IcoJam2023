using DG.Tweening;
using Temporal;

namespace Traps
{
    public class DoorTemporal : DependantBoolTemporal
    {
        private Tween _rightDoorPositionTween;
        private Tween _leftDoorPositionTween;
        private Tween _rightDoorScaleTween;
        private Tween _leftDoorScaleTween;
        private Tween _rightDoorMiddleTween;
        private Tween _leftDoorMiddleTween;
        private bool _open;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (Triggered)
            {
                Open();
            } else
            {
                Close();
            }
        }

        private void Open()
        {
            if (_open)
            {
                return;
            }

            _open = true;
            
            // Door move
            _rightDoorPositionTween?.Kill();
            _rightDoorPositionTween = transform.GetChild(0).DOLocalMoveX(1f, 0.2f)
                .SetEase(Ease.Linear);
            _leftDoorPositionTween?.Kill();
            _leftDoorPositionTween = transform.GetChild(1).DOLocalMoveX(-1f, 0.2f)
                .SetEase(Ease.Linear);
            
            // Door scale
            _rightDoorScaleTween?.Kill();
            _rightDoorScaleTween = transform.GetChild(0).DOScaleY(0f, 0.2f)
                .SetEase(Ease.Linear);
            _leftDoorScaleTween?.Kill();
            _leftDoorScaleTween = transform.GetChild(1).DOScaleY(0f, 0.2f)
                .SetEase(Ease.Linear);
            
            // Door middle
            _rightDoorMiddleTween?.Kill();
            _rightDoorMiddleTween = transform.GetChild(2).DOLocalMoveX(.95f, 0.2f)
                .SetEase(Ease.Linear);
            _leftDoorMiddleTween?.Kill();
            _leftDoorMiddleTween = transform.GetChild(3).DOLocalMoveX(-.95f, 0.2f)
                .SetEase(Ease.Linear);

            if (!Reversing)
            {
                AudioManager.Play("door-open");
            }
        }
        
        private void Close()
        {
            if (!_open)
            {
                return;
            }

            _open = false;
            // Door move
            _rightDoorPositionTween?.Kill();
            _rightDoorPositionTween = transform.GetChild(0).DOLocalMoveX(0f, 0.2f)
                .SetEase(Ease.Linear);
            _leftDoorPositionTween?.Kill();
            _leftDoorPositionTween = transform.GetChild(1).DOLocalMoveX(0f, 0.2f)
                .SetEase(Ease.Linear);
            
            
            // Door scale
            _rightDoorScaleTween?.Kill();
            _rightDoorScaleTween = transform.GetChild(0).DOScaleY(100, 0.2f)
                .SetEase(Ease.Linear);
            _leftDoorScaleTween?.Kill();
            _leftDoorScaleTween = transform.GetChild(1).DOScaleY(100, 0.2f)
                .SetEase(Ease.Linear);
            
            // Door middle
            _rightDoorMiddleTween?.Kill();
            _rightDoorMiddleTween = transform.GetChild(2).DOLocalMoveX(0f, 0.2f)
                .SetEase(Ease.Linear);
            _leftDoorMiddleTween?.Kill();
            _leftDoorMiddleTween = transform.GetChild(3).DOLocalMoveX(0f, 0.2f)
                .SetEase(Ease.Linear);
            
            if (!Reversing)
            {
                AudioManager.Play("door-close");
            }
        }
    }
}