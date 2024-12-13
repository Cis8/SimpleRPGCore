using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Events {
    [CreateAssetMenu(fileName = "New Game Event", menuName = "Simple RPG Core/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        // evaluate if this shall be changed to a fixed size array that is resized when needed
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListener> _listeners = new();

        public void Raise() {
            for(var i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
    }
}
