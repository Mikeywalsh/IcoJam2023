using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Temporal;
using UnityEngine;

namespace Traps
{
    public class ButtonTemporal : BoolTemporal
    {
        private MeshRenderer _buttonHolderMeshRenderer;
        private int _indicatorMaterialIndex = 1;
        public GameObject ButtonHolder;
        public GameObject ButtonModel;
        private Material[] _buttonHolderMaterials;

        private Tween _buttonModelTween;
        public Material OnMaterial;
        public Material OffMaterial;
        private bool _wasLockedLastFrame;
        private readonly HashSet<GameObject> _standingObjects = new();
        
        protected override void Start()
        {
            base.Start();
            _buttonHolderMeshRenderer = ButtonHolder.GetComponent<MeshRenderer>();
            _buttonHolderMaterials = _buttonHolderMeshRenderer.materials;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (Triggered && _wasLockedLastFrame && _standingObjects.Count == 0)
            {
                TryTurnOff();
            }

            _wasLockedLastFrame = IsLocked();
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();
            
            _buttonModelTween?.Kill();
            _buttonModelTween = ButtonModel.transform.DOLocalMoveZ(Triggered ? -0.00178f : 0f, 0.2f);
            _buttonHolderMaterials[_indicatorMaterialIndex] = Triggered ? OnMaterial : OffMaterial;
            _buttonHolderMeshRenderer.materials = _buttonHolderMaterials;
        }

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