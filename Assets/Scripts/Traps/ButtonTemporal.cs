using System;
using System.Collections.Generic;
using System.Linq;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class ButtonTemporal : BoolTemporal
    {
        private MeshRenderer _meshRenderer;

        public Material OnMaterial;
        public Material OffMaterial;
        private bool _wasLockedLastFrame;

        private readonly List<GameObject> _standingObjects = new();
        
        protected override void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void FixedUpdate()
        {
            _meshRenderer.material = Triggered ? OnMaterial : OffMaterial;
            if (Triggered && _wasLockedLastFrame && _standingObjects.Count == 0)
            {
                TryTurnOff();
            }

            _wasLockedLastFrame = IsLocked();
        }

        // Make a collision map to not be triggered by projectiles
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            _standingObjects.Add(other.gameObject);
            
            TryTurnOn();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            TryTurnOn();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            
            _standingObjects.Remove(other.gameObject);

            if (_standingObjects.Count == 0)
            {
                TryTurnOff();
            }
        }
    }
}