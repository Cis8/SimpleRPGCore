using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using UnityEngine;
using UnityEngine.Events;

namespace ElectricDrill.SimpleRpgCore.Events {
    public class GameEventListenerGeneric3<T, U, W> : MonoBehaviour
    {
        [SerializeField] protected GameEventGeneric3<T, U, W> _event;
        [SerializeField] protected UnityEvent<T, U, W> _response;

        private void OnEnable() {
            _event.RegisterListener(this);
        }
        
        private void OnDisable() {
            _event.UnregisterListener(this);
        }

        public void OnEventRaised(T contextT, U contextU, W contextW) {
            _response.Invoke(contextT, contextU, contextW);
        }
    }
}