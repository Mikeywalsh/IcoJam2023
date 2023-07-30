using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class SwitchTemporal : BoolTemporal
    {
        private List<MeshRenderer> _meshRenderers;
        private Transform _toggle;
        private bool _toggled;
        private Tween _toggleTween;

        public Material OnMaterial;
        public Material OffMaterial;

        private void Start()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        }
        
        private void FixedUpdate()
        {
            _meshRenderers.ForEach(meshRenderer => meshRenderer.material = Triggered ? OnMaterial : OffMaterial);
            
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
            _toggleTween = transform.GetChild(0).DOLocalRotate(new Vector3(130f, 0f, 0f), 0.2f)
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
            _toggleTween = transform.GetChild(0).DOLocalRotate(new Vector3(-130f, 0f, 0f), 0.2f)
                .SetEase(Ease.Linear);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            TryToggle();
            OnInteractedWith();
        }
    }
}