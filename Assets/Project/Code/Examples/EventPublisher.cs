using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UnityEngine;

namespace Project.Code.Examples
{
    public class EventPublisher : MonoBehaviour
    {
        [SerializeField] private KeyCode keyToPress;
        
        private void Update()
        {
            if (Input.GetKeyDown(keyToPress))
            {
                var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
                var signal = new DummyEvent { PressedKey = keyToPress };
                
                dispatcher.Trigger<DummyEvent>(signal);
            }
        }
    }
}