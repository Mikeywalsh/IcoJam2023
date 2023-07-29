using DG.Tweening;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class DoorTemporal : DependantBoolTemporal
    {
        private Tween _rightTween;
        private Tween _leftTween;
        private bool _open;

        public override int ExecutionOrder() => 1;

        private void FixedUpdate()
        {
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
            _rightTween?.Kill();
            _rightTween = transform.GetChild(0).DOLocalMoveX(1.5f, 0.2f)
                .SetEase(Ease.Linear);
            _leftTween?.Kill();
            _leftTween = transform.GetChild(1).DOLocalMoveX(-1.5f, 0.2f)
                .SetEase(Ease.Linear);
        }
        
        private void Close()
        {
            if (!_open)
            {
                return;
            }

            _open = false;
            _rightTween?.Kill();
            _rightTween = transform.GetChild(0).DOLocalMoveX(0.5f, 0.2f)
                .SetEase(Ease.Linear);
            _leftTween?.Kill();
            _leftTween = transform.GetChild(1).DOLocalMoveX(-0.5f, 0.2f)
                .SetEase(Ease.Linear);
        }
    }
}