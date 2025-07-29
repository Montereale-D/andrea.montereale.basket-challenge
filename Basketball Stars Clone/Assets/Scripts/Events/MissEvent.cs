using Enums;

namespace Events
{
    public class MissEvent : EventData
    {
        public PlayerNumber Player { get; private set; }

        public MissEvent(PlayerNumber player)
        {
            Player = player;
        }
    }
}