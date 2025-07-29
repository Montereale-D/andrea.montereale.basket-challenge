using UnityEngine;

namespace Gameplay.Bonus
{
    /// <summary>
    /// ScriptableObject containing configuration settings for in-game bonuses.
    /// Includes durations and spawn chances for common, rare, and very rare bonuses,
    /// as well as a cooldown time between bonus activations.
    /// </summary>
    [CreateAssetMenu(fileName = "BonusSettings", menuName = "Gameplay/Bonus Settings")]
    public class BonusSettings : ScriptableObject
    {
        [Header("Backboard Bonus Duration")]
        [SerializeField] public float commonBonusDuration = 5f;
        [SerializeField] public float rareBonusDuration = 8f;
        [SerializeField] public float veryRareBonusDuration = 12f;
    
        [Header("Backboard Bonus Chance")]
        [SerializeField, Range(0f, 1f)] public float commonBonusChance = 0.04f;
        [SerializeField, Range(0f, 1f)] public float rareBonusChance = 0.06f;
        [SerializeField, Range(0f, 1f)] public float veryRareBonusChance = 0.08f;
    
        [Header("Other Bonus Settings")]
        [SerializeField] public float bonusCooldown = 10f;
    }
}