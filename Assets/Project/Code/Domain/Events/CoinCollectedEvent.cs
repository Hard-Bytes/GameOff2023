using Project.Code.Patterns.Events;

namespace Project.Code.Domain.Events
{
    public class CoinCollectedEvent : GameEvent
    {
        public int points;
        public bool isBigCoin;
    }
}