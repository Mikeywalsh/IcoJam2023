using System;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class ButtonTemporal : BoolTemporal
    {
        private MeshRenderer _meshRenderer;

        public Material OnMaterial;
        public Material OffMaterial;
        
        protected override void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void FixedUpdate()
        {
            _meshRenderer.material = Triggered ? OnMaterial : OffMaterial;
        }

        // Make a collision map to not be triggered by projectiles
        private void OnTriggerEnter(Collider other)
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            TurnOn();
            OnInteractedWith();
        }

        private void OnTriggerStay(Collider other)
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            OnInteractedWith();
        }

        private void OnTriggerExit(Collider other)
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }

            OnInteractedWith();
            TurnOff();
        }
    }
}