﻿using System.Collections.Generic;
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
        private readonly List<GameObject> _standingObjects = new();
        
        protected override void Start()
        {
            base.Start();
            _buttonHolderMeshRenderer = ButtonHolder.GetComponent<MeshRenderer>();
            _buttonHolderMaterials = _buttonHolderMeshRenderer.materials;
        }
        protected override void OnStateChanged()
        {
            base.OnStateChanged();
            
            _buttonModelTween?.Kill();
            _buttonModelTween = ButtonModel.transform.DOLocalMoveZ(Triggered ? -0.00178f : 0f, 0.2f);
            _buttonHolderMaterials[_indicatorMaterialIndex] = Triggered ? OnMaterial : OffMaterial;
            _buttonHolderMeshRenderer.materials = _buttonHolderMaterials;

            if (!Reversing)
            {
                PlaySound(Triggered);
            }
        }
        
        public override void StartedReversing()
        {
            base.StartedReversing();

            if (_standingObjects.Count == 1 && _standingObjects[0].GetComponent<PlayerTemporal>())
            {
                TryTurnOff();
            }
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

        private void TryTurnOn()
        {
            if (Reversing || IsLocked())
            {
                return;
            }

            var wasOn = Triggered;
            
            Triggered = true;
            OnInteractedWith();
            if (!wasOn)
            {
                OnStateChanged();
            }
        }

        private void TryTurnOff()
        {
            if (Reversing || IsLocked() || _standingObjects.Count != 0)
            {
                return;
            }
            Triggered = false;
            OnInteractedWith();
            OnStateChanged();
        }

        private void PlaySound(bool on)
        {
            AudioManager.Play(on ? "button-on" : "button-off");
        }
    }
}