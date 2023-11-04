using System;
using System.Collections.Generic;

namespace Project.Code.Patterns.Events
{
    /// <summary>
    /// Global Game Event Dispatcher, it invokes the methods it has subscribed to, can only trigger them instantly
    /// </summary>
    public class EventDispatcherImpl : EventDispatcher
    {
        private readonly Dictionary<Type, Action<GameEvent>> _eventsToDelegatesDictionary;

        public EventDispatcherImpl()
        {
            _eventsToDelegatesDictionary = new Dictionary<Type, Action<GameEvent>>();
        }
        
        public void Subscribe<T>(Action<GameEvent> callback) where T : GameEvent
        {
            var type = typeof(T);
            
            if (!_eventsToDelegatesDictionary.ContainsKey(type)) _eventsToDelegatesDictionary.Add(type, null);

            _eventsToDelegatesDictionary[type] += callback;
        }

        public void Unsubscribe<T>(Action<GameEvent> callback) where T : GameEvent
        {
            var type = typeof(T);

            if (_eventsToDelegatesDictionary.ContainsKey(type))
            {
                _eventsToDelegatesDictionary[type] -= callback;
            }
        }

        public void Trigger<T>(GameEvent signal) where T : GameEvent
        {
            var type = typeof(T);

            if (!_eventsToDelegatesDictionary.TryGetValue(type, out var callback)) return;
            
            callback.Invoke(signal);
        }
    }
}