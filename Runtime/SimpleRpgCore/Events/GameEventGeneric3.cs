using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Events {
    public abstract class GameEventGeneric3<T, U, W> : ScriptableObject
    {
        // evaluate if this shall be changed to a fixed size array that is resized when needed
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerGeneric3<T, U, W>> _listeners = new();
        private readonly List<Action<T, U, W>> _codeListeners = new();

        public event Action<T, U, W> OnEventRaised {
            add => RegisterListener(value);
            remove => UnregisterListener(value);
        }
        
        public void Raise(T contextT, U contextU, W contextW) {
            for(var i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnEventRaised(contextT, contextU, contextW);
            }
            
            for(var i = _codeListeners.Count - 1; i >= 0; i--) {
                _codeListeners[i].Invoke(contextT, contextU, contextW);
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
        
        private void RegisterListener(Action<T, U, W> listener)
        {
            if (!_codeListeners.Contains(listener))
                _codeListeners.Add(listener);
        }
        
        private void UnregisterListener(Action<T, U, W> listener)
        {
            if (_codeListeners.Contains(listener))
                _codeListeners.Remove(listener);
        }
    }
}