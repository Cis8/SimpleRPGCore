using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Events {
    public abstract class GameEventGeneric1<T> : ScriptableObject
    {
        // evaluate if this shall be changed to a fixed size array that is resized when needed
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerGeneric1<T>> _listeners = new();
        
        private readonly List<Action<T>> _codeListeners = new();

        public event Action<T> OnEventRaised {
            add => RegisterListener(value);
            remove => UnregisterListener(value);
        }

        public void Raise(T context) {
            for(var i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnEventRaised(context);
            }
            
            for(var i = _codeListeners.Count - 1; i >= 0; i--) {
                _codeListeners[i].Invoke(context);
            }
        }

        public void RegisterListener(GameEventListenerGeneric1<T> listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerGeneric1<T> listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
        
        private void RegisterListener(Action<T> listener)
        {
            if (!_codeListeners.Contains(listener))
                _codeListeners.Add(listener);
        }
        
        private void UnregisterListener(Action<T> listener)
        {
            if (_codeListeners.Contains(listener))
                _codeListeners.Remove(listener);
        }
    }
}
