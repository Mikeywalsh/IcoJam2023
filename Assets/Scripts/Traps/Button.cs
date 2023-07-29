using System;
using Temporal;
using UnityEngine;

namespace Traps
{
    [RequireComponent(typeof(BoolTemporal))]
    public class Button : MonoBehaviour
    {
        //private SpriteRenderer _spriteRenderer;
        private BoolTemporal _boolTemporal;
        
        private void Start()
        {
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            _boolTemporal = GetComponent<BoolTemporal>();
        }

        /*
        private void FixedUpdate()
        {
            _spriteRenderer.color = _boolTemporal.Triggered ? Color.green : Color.red;
        }*/

        // Make a collision map to not be triggered by projectiles
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            _boolTemporal.TurnOn();
            _boolTemporal.OnInteractedWith();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            _boolTemporal.OnInteractedWith();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<ITemporal>() == null)
            {
                return;
            }
            _boolTemporal.TurnOff();
        }
    }
}