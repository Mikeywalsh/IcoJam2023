using Temporal;
using UnityEngine;

namespace Traps
{
    [RequireComponent(typeof(BoolTemporal))]
    public class Switch : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private BoolTemporal _boolTemporal;

        public Material OnMaterial;
        public Material OffMaterial;
        
        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _boolTemporal = GetComponent<BoolTemporal>();
        }
        
        private void FixedUpdate()
        {
            _meshRenderer.material = _boolTemporal.Triggered ? OnMaterial : OffMaterial;
        }

        // Make a collision map to not be triggered by projectiles
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            _boolTemporal.Toggle();
            _boolTemporal.OnInteractedWith();
        }
    }
}