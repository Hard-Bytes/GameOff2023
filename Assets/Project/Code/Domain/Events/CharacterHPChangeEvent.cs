using Project.Code.Patterns.Events;

namespace Project.Code.Domain.Events
{
    public struct CharacterHPChangeEvent : GameEvent
    {
        public int NewHP;
        public int NewDivision;
        public int MaxHP;
    }
}