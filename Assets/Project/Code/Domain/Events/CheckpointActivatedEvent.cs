using UnityEngine;
using Project.Code.Patterns.Events;

namespace Project.Code.Domain.Events
{
    public struct CheckpointActivatedEvent : GameEvent
    {
        public Vector2 RespawnPosition;
        public SlimeSize RespawnSize;
        public bool IsStartOfLevel;
    }
}
