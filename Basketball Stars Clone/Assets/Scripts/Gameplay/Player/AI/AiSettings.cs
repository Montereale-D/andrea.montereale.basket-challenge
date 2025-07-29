using UnityEngine;

namespace Gameplay.Player.AI
{
    /// <summary>
    /// ScriptableObject containing AI behavior settings.
    /// Used to configure AI player difficulty and decision-making patterns.
    /// </summary>
    [CreateAssetMenu(fileName = "AiSettings", menuName = "Gameplay/AI Settings")]
    public class AiSettings : ScriptableObject
    {
        public float reactionTime = 5f;
        public bool reactionToBonus;
        [Range(0f, 1f)] public float perfectChance = 0.04f;
        [Range(0f, 1f)] public float backboardChance = 0.06f;
        [Range(0f, 1f)] public float noPerfectChance = 0.08f;
        [Range(0f, 1f)] public float noBackboardChance = 0.08f;
    }
}