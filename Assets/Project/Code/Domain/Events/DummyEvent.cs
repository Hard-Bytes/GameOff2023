using UnityEngine;
using Project.Code.Patterns.Events;

namespace Project.Code.Domain.Events
{
    public struct DummyEvent : GameEvent
    {
        public KeyCode PressedKey;
    }
}