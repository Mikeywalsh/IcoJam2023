using System.Collections.Generic;
using System.Linq;
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
        
        private readonly HashSet<GameObject> _standingObjects = new();

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
            _standingObjects.Add(other.gameObject);

            if (_standingObjects.Count > 0)
            {
                TryToggle();
            }
        }
        
        protected void TryToggle()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            _triggerCollider.enabled = false;
            Triggered = !Triggered;
            OnStateChanged();
            OnInteractedWith();
            PlaySound(Triggered);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            _standingObjects.Remove(other.gameObject);
        }
    }
}