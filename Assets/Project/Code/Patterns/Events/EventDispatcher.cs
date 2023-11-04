using System;

namespace Project.Code.Patterns.Events
{
    public interface EventDispatcher
    {
        void Subscribe<T>(Action<GameEvent> callback) where T : GameEvent;
        void Unsubscribe<T>(Action<GameEvent> callback) where T : GameEvent;
        void Trigger<T>(GameEvent signal) where T : GameEvent;
    }
}