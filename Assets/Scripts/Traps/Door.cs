using System.Collections.Generic;
using DG.Tweening;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class Door : MonoBehaviour
    {
        [SerializeField]
        private List<BoolTemporal> _boolTemporals;

        private bool _open;
        private Tween _rightTween;
        private Tween _leftTween;

        private void FixedUpdate()
        {
            if (_boolTemporals.TrueForAll(temporal => temporal.Triggered))
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