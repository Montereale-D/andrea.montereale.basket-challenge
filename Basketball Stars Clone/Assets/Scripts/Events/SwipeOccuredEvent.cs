using Enums;

namespace Events
{
    public class SwipeOccuredEvent : EventData
    {
        public PlayerNumber Player {get; private set;}
        public float Value { get; private set; }
        public SwipeOccuredEvent(PlayerNumber player, float value)
        {
            Player = player;
            Value = value;
        }
    }
}