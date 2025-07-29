using Data;
using Enums;
using UnityEngine;

namespace Events
{
    public class ThrowSpotLoadedEvent : EventData
    {
        public PlayerNumber Player { get; private set; }
        public ThrowSpotData ThrowSpotData { get; private set; }
        public Vector3 StartPos { get; private set; }

        public ThrowSpotLoadedEvent(PlayerNumber player, ThrowSpotData throwSpotData, Vector3 startPos)
        {
            Player = player;
            this.ThrowSpotData = throwSpotData;
            this.StartPos = startPos;
        }
    }
}