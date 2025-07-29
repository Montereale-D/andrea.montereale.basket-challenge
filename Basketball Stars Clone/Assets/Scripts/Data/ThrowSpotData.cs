using UnityEngine;

namespace Data
{
    /// <summary>
    /// Represent data of a throw spot.
    /// </summary>
    [CreateAssetMenu(fileName = "ThrowSpotData", menuName = "Gameplay/Throw Spot Data")]
    public class ThrowSpotData : ScriptableObject
    {
        public SwipeAccuracyData perfectAccuracyData;
        public SwipeAccuracyData backboardAccuracyData;
        public float throwAngle = 60;
    }
}