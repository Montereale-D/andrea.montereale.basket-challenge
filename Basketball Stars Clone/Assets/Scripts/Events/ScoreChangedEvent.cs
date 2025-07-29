using Enums;

namespace Events
{
    public class ScoreChangedEvent : EventData
    {
        public float Score { get; set; }
        public TargetType Target { get; set; }

        public ScoreChangedEvent(float score, TargetType target)
        {
            Score = score;
            Target = target;
        }
    }
}
