using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UnityEngine;

namespace Project.Code.Examples
{
    public class EventConsumer : MonoBehaviour
    {
        private void Awake()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            
            dispatcher.Subscribe<DummyEvent>(OnDummyEvent);
        }

        private void OnDestroy()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            
            dispatcher.Unsubscribe<DummyEvent>(OnDummyEvent);
        }

        private void OnDummyEvent(GameEvent signal)
        {
            Debug.LogWarning("I received an event");
        }
    }
}