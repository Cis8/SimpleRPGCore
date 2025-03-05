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
                Debug.LogWarning("Entity is null for HeroUIController of " + name);
                return;
            }
            if (entity.gameObject.TryGetComponent<Class>(out var @class))
                className.text = @class.name;
        }
    }
}
