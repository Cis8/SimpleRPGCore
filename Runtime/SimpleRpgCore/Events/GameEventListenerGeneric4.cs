using UnityEngine;
using UnityEngine.Events;

namespace ElectricDrill.SimpleRpgCore.Events {
    public class GameEventListenerGeneric4<T, U, W, K> : MonoBehaviour
    {
        [SerializeField] protected GameEventGeneric4<T, U, W, K> _event;
        [SerializeField] protected UnityEvent<T, U, W, K> _response;

        private void OnEnable() {
            _event.RegisterListener(this);
        }
        
        private void OnDisable() {
            _event.UnregisterListener(this);
        }

        public void OnEventRaised(T contextT, U contextU, W contextW, K contextK) {
            _response.Invoke(contextT, contextU, contextW, contextK);
        }
    }
}