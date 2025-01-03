using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Events {
    public abstract class GameEventGeneric2<T, U> : ScriptableObject, IRaisable<T, U>
    {
        // evaluate if this shall be changed to a fixed size array that is resized when needed
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerGeneric2<T, U>> _listeners = new();
        private readonly List<Action<T, U>> _codeListeners = new();
        
        public event Action<T, U> OnEventRaised {
            add => RegisterListener(value);
            remove => UnregisterListener(value);
        }

        public void Raise(T context1, U context2) {
            for (var i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnEventRaised(context1, context2);
            }

            for (var i = _codeListeners.Count - 1; i >= 0; i--) {
                _codeListeners[i].Invoke(context1, context2);
            }
        }

        public void RegisterListener(GameEventListenerGeneric2<T, U> listener) {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerGeneric2<T, U> listener) {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }

        private void RegisterListener(Action<T, U> listener) {
            if (!_codeListeners.Contains(listener))
                _codeListeners.Add(listener);
        }

        private void UnregisterListener(Action<T, U> listener) {
            if (_codeListeners.Contains(listener))
                _codeListeners.Remove(listener);
        }
    }
}