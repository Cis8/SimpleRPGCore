using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore;
using TMPro;
using UnityEngine;

namespace ElectricDrill {
    public class HeroUIController : MonoBehaviour
    {
        public EntityCore entity;

        public TextMeshProUGUI className;
        
        void Start() {
            
        }

        void Update() {
            
        }

        private void OnValidate() {
            if (entity == null) {
                return;
            }
            if (entity.gameObject.TryGetComponent<Class>(out var @class))
                className.text = @class.name;
        }
    }
}
