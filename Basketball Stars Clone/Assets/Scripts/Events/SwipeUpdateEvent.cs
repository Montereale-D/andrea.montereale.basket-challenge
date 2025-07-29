namespace Events
{
    public class SwipeUpdateEvent : EventData
    {
        public float Value { get; protected set; }
        public SwipeUpdateEvent(float value)
        {
            Value = value;
        }
    }
}