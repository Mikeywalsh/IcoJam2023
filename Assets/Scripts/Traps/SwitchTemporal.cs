using DG.Tweening;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class SwitchTemporal : BoolTemporal
    {
        private MeshRenderer _handleMeshRenderer;
        private Transform _toggleTransform;
        private bool _toggled;
        private Tween _toggleTween;

        public Material OnMaterial;
        public Material OffMaterial;
        private bool _wasLockedLastFrame;

        public Collider _triggerCollider;
        private int _toggledFrame;


        protected override void Start()
        {
            base.Start();
            _triggerCollider = GetComponent<Collider>();
            _toggleTransform = transform.GetChild(0);
            _handleMeshRenderer = _toggleTransform.GetComponent<MeshRenderer>();
            _toggled = !Triggered;
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            _handleMeshRenderer.material = Triggered ? OnMaterial : OffMaterial;
            if (Triggered)
            {
                MoveToggleOn();
            } else
            {
                MoveToggleOff();
            }
        }
        
        private void MoveToggleOn()
        {
            if (_toggled)
            {
                return;
            }

            _toggled = true;
            _toggleTween?.Kill();
            _toggleTween = _toggleTransform.DOLocalRotate(new Vector3(-55f, 0f, 0f), 0.1f)
                .SetEase(Ease.Linear);
        }
        
        private void MoveToggleOff()
        {
            if (!_toggled)
            {
                return;
            }

            _toggled = false;
            _toggleTween?.Kill();
            _toggleTween = _toggleTransform.DOLocalRotate(new Vector3(-125f, 0f, 0f), 0.1f)
                .SetEase(Ease.Linear);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }

            TryToggle();
        }


        protected override bool ShouldDisplayLockedIcon()
        {
            return _toggledFrame > CurrentFrame;
        }

        private void TryToggle()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            _triggerCollider.enabled = false;
            Triggered = !Triggered;
            OnStateChanged();
            PlaySound(Triggered);

            _toggledFrame = CurrentFrame;
            // Toggle only works once, fill the rest of the buffer with current value
            LockedEnd = TemporalBuffer.Length;
            for (var i = CurrentFrame; i < LockedEnd; i++)
            {
                TemporalBuffer[i] = new BoolTemporalState(Triggered);
            }
        }
    }
}