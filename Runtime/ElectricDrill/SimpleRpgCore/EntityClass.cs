using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElectricDrill.SimpleRpgCore {
    public class EntityClass : MonoBehaviour
    {
        [SerializeField] private Class _class;
        
        public Class Class => _class;

        private void Awake() {
            Assert.IsNotNull(_class, $"Class of {gameObject.name} is missing");
        }

        void Start() {
            
        }

        void Update() {
            
        }

        public static implicit operator Class(EntityClass entityClass) => entityClass.Class;
    }
}
