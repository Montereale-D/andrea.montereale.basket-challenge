using Enums;

namespace Events
{
    public class BonusStartEvent : EventData
    {
        public BonusRarity Rarity { get; set; }

        public BonusStartEvent(BonusRarity rarity)
        {
            Rarity = rarity;
        }
    }
}