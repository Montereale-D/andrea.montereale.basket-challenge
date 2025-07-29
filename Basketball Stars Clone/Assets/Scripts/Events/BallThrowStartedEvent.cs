using Enums;

namespace Events
{
    public class BallThrowStartedEvent : EventData
    {
        public PlayerNumber PlayerNumber;

        public BallThrowStartedEvent(PlayerNumber playerNumber)
        {
            PlayerNumber = playerNumber;
        }
    }
}