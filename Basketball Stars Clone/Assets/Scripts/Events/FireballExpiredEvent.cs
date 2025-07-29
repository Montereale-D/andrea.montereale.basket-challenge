using Enums;

namespace Events
{
    public class FireballExpiredEvent : EventData
    {
        public PlayerNumber Player { get; private set; }

        public FireballExpiredEvent(PlayerNumber player)
        {
            Player = player;
        }
    }
}