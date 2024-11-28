using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace ElectricDrill.SimpleRpgCore.Events {
    public class GameEventListenerGeneric1<T> : MonoBehaviour
    {
        [SerializeField] protected GameEventGeneric1<T> _event;
        [SerializeField] protected UnityEvent<T> _response;

        private void OnEnable() {
            _event.RegisterListener(this);
        }
        
        private void OnDisable() {
            _event.UnregisterListener(this);
        }

        public void OnEventRaised(T context) {
            _response.Invoke(context);
        }
    }
}
