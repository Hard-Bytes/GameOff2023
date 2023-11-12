using Project.Code.Patterns.Events;

namespace Project.Code.Domain.Events
{
    public struct CharacterHPChangeEvent : GameEvent
    {
        public int NuevaHP;
        public int NuevaDivision;
        public int VidaMaxima;
    }
}