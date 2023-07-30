using System.Collections.Generic;
using System.Linq;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class LaserCannonTemporal : DependantBoolTemporal
    {
        private List<LineRenderer> _lineRenderers;
        private List<Light> _pointLights;

        public override int ExecutionOrder() => 1;
        
        protected override void Start()
        {
            _lineRenderers = GetComponentsInChildren<LineRenderer>(true).ToList();
            _pointLights = GetComponentsInChildren<Light>(true).ToList();
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (Triggered)
            {
                TurnLaserOn();
            }
            else
            {
                TurnLaserOff();
            }
        }
        
        private void Update()
        {
            if (Triggered)
            {
                foreach (var lineRenderer in _lineRenderers)
                {
                    if (Physics.Raycast(lineRenderer.transform.position, lineRenderer.transform.forward, out var hit, 
                        Mathf.Infinity, int.MaxValue, QueryTriggerInteraction.Ignore))
                    {
                        lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));
                        Debug.DrawRay(lineRenderer.transform.position, lineRenderer.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);

                        if (hit.collider.gameObject.GetComponentInParent<PlayerController>())
                        {
                            var playerObject = hit.collider.gameObject.GetComponentInParent<PlayerController>();
                            playerObject.Die();
                        }
                    }
                    else
                    {
                        lineRenderer.SetPosition(1, new Vector3(0, 0, 69));
                    }
                }
            }
        }
        
        private void TurnLaserOn()
        {
            _lineRenderers.ForEach(lineRenderer => lineRenderer.enabled = true);
            _pointLights.ForEach(pointLight => pointLight.enabled = true);
        }

        private void TurnLaserOff()
        {
            _lineRenderers.ForEach(lineRenderer => lineRenderer.enabled = false);
            _pointLights.ForEach(pointLight => pointLight.enabled = false);
        }
    }
}