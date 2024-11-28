using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Events {
    public abstract class GameEventGeneric3<T, U, W> : ScriptableObject
    {
        // evaluate if this shall be changed to a fixed size array that is resized when needed
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerGeneric3<T, U, W>> _listeners = new();

        public void Raise(T contextT, U contextU, W contextW) {
            for(var i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnEventRaised(contextT, contextU, contextW);
            }
        }

        public void RegisterListener(GameEventListenerGeneric3<T, U, W> listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerGeneric3<T, U, W> listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
    }
}