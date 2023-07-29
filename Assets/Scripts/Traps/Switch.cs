using System.Collections.Generic;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class Switch : BoolTemporal
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            Toggle();
            OnInteractedWith();
        }
    }
}