using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElectricDrill.SimpleRpgCore {
    public class EntityClass : MonoBehaviour, IClassSource
    {
        [SerializeField] private Class _class;
        
        public Class Class {
            get => _class;
            internal set => _class = value;
        }

        private void Awake() {
        }

        void Start() {
            Assert.IsNotNull(_class, $"Class of {gameObject.name} is missing");
        }

        void Update() {
            
        }

        public static implicit operator Class(EntityClass entityClass) => entityClass.Class;
    }
}
