using Project.Code.Patterns.Events;

namespace Project.Code.Domain.Events
{
    public struct LevelWinConditionMetEvent : GameEvent
    {
        // Añadimos un enum de condiciones? de momento no hace falta porque la única va a ser llegar al punto B
        public string SceneToSwitch;
    }
}