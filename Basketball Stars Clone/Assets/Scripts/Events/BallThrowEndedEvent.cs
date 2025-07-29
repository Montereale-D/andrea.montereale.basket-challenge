using Enums;

namespace Events
{
    public class BallThrowEndedEvent : EventData
    {
        public PlayerNumber PlayerNumber;

        public BallThrowEndedEvent(PlayerNumber playerNumber)
        {
            PlayerNumber = playerNumber;
        }
    }
}