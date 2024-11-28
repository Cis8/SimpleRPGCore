using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using UnityEngine;
using UnityEngine.Events;

namespace ElectricDrill.SimpleRpgCore.Events {
    public class GameEventListenerGeneric2<T, U> : MonoBehaviour
    {
        [SerializeField] protected GameEventGeneric2<T, U> _event;
        [SerializeField] protected UnityEvent<T, U> _response;

        private void OnEnable() {
            _event.RegisterListener(this);
        }
        
        private void OnDisable() {
            _event.UnregisterListener(this);
        }

        public void OnEventRaised(T contextT, U contextU) {
            _response.Invoke(contextT, contextU);
        }
    }
}