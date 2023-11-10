using Project.Code.Patterns.Events;
using Project.Code.Utils;

namespace Project.Code.Domain.Events
{
    public struct CharacterDiedEvent : GameEvent
    {
        public DamageSource Source;
    }
}
