using Enums;

namespace Events
{
    public class FireballStartEvent : EventData
    {
        public PlayerNumber Player { get;  private set;}
        public float Duration { get;  private set;}

        public FireballStartEvent(PlayerNumber player, float duration)
        {
            Player = player;
            Duration = duration;
        }
    }
}