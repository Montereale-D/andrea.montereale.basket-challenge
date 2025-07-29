using System.Collections.Generic;
using Enums;
using Events;
using Interfaces;
using UI.Gameplay;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Handles scoring logic and fireball triggers during gameplay.
    /// Tracks player scores, streaks and activates special scoring modes (fireball and bonus multipliers).
    /// Integrates with UI controllers to display score and fireball visuals.
    /// </summary>

    [RequireComponent(typeof(FireballUIController))]
    [RequireComponent(typeof(ScoreUIController))]
    public class ScoreManager : MonoBehaviour, IScoreDataService
    {
        [Header("Base score settings")]
        [SerializeField] private int defaultScore = 2;
        [SerializeField] private int backboardScore = 2;
        [SerializeField] private int perfectScore = 3;
    
        [Header("Fireball Settings")]
        [SerializeField] private int fireballStreakThreshold = 2;
        [SerializeField] private float fireballDuration = 10f;
    
        [Header("Bonus Score Settings")]
        [SerializeField] private int commonBonus = 4;
        [SerializeField] private int rareBonus = 6;
        [SerializeField] private int veryRareBonus = 8;

        private static ScoreManager Instance { get; set; }
        
        private FireballUIController _fireballUIController;
        private ScoreUIController _scoreUIController;
        
        private readonly Dictionary<PlayerNumber, int> _scores = new();
        private readonly Dictionary<PlayerNumber, int> _streaks = new();
        private readonly Dictionary<PlayerNumber, bool> _enabledFireballs = new();

        public int ActiveBonus { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            ServiceLocator.ScoreDataService = this;
            
            _fireballUIController = GetComponent<FireballUIController>();
            _scoreUIController = GetComponent<ScoreUIController>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<TimeEndEvent>(OnTimeEnd);
            EventBus.Subscribe<GameStartEvent>(ResetScore);
            EventBus.Subscribe<FireballExpiredEvent>(DeactivateFireball);
            EventBus.Subscribe<BonusStartEvent>(BonusStart);
            EventBus.Subscribe<BonusEndEvent>(BonusStop);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<TimeEndEvent>(OnTimeEnd);
            EventBus.Unsubscribe<GameStartEvent>(ResetScore);
            EventBus.Unsubscribe<FireballExpiredEvent>(DeactivateFireball);
            EventBus.Unsubscribe<BonusStartEvent>(BonusStart);
            EventBus.Unsubscribe<BonusEndEvent>(BonusStop);
        }

        public void ThrowResult(PlayerNumber player, TargetType target, bool result)
        {
            _scores.TryAdd(player, 0);
            _streaks.TryAdd(player, 0);
            _enabledFireballs.TryAdd(player, false);
        
            if (!result)
            {
                OnScoreMiss(player);
            }
            else
            {
                int bonusScore = GetBonusScore(target);
                int score = (GetBaseScore(target) + bonusScore) * GetScoreMultiplier(player);
                AddScore(player, score);
                AddStreak(player);

                if (player == PlayerNumber.Player1)
                {
                    EventBus.Raise(new ScoreChangedEvent(score, target));

                    if (score > 0)
                    {
                        if (target == TargetType.Perfect)
                        {
                            ServiceLocator.SoundService.PlaySound(SoundType.SFX_SCORE_PERFECT);
                        }
                        
                        if (target == TargetType.Backboard && bonusScore > 0)
                        {
                            ServiceLocator.SoundService.PlaySound(SoundType.SFX_BONUS);
                        }
                    }
                }
            }
        }

        private void OnScoreMiss(PlayerNumber player)
        {
            _fireballUIController.DeactivateFireball(player);
            ResetStreak(player);
            EventBus.Raise(new MissEvent(player));
        }

        private void ActivateFireball(PlayerNumber player)
        {
            _enabledFireballs[player] = true;
            _fireballUIController.ActivateFireball(player);
            EventBus.Raise(new FireballStartEvent(player, fireballDuration));
        }
        
        private void DeactivateFireball(FireballExpiredEvent args)
        {
            var player = args.Player;
            DeactivateFireball(player);
        }
        
        private void DeactivateFireball(PlayerNumber player)
        {
            _enabledFireballs[player] = false;
            _fireballUIController.DeactivateFireball(player);
        }
        
        private void OnTimeEnd(TimeEndEvent evt)
        {
            ResetStreaks();
            _fireballUIController.Hide();
        }

        private void BonusStart(BonusStartEvent args)
        {
            ActiveBonus = args.Rarity switch
            {
                BonusRarity.Common => commonBonus,
                BonusRarity.Rare => rareBonus,
                BonusRarity.VeryRare => veryRareBonus,
                _ => ActiveBonus
            };
        }

        private void BonusStop()
        {
            ActiveBonus = 0;
        }
        
        private void ResetScore()
        {
            _scores.Clear();
            _streaks.Clear();
            _enabledFireballs.Clear();
            
            ResetStreaks();
            
            _scoreUIController.ResetScore();
            _scoreUIController.ShowScores();
            
            _fireballUIController.SetThreshold(fireballStreakThreshold);
            _fireballUIController.ResetStreak();
            _fireballUIController.Show();
            
            BonusStop();
        }

        private int GetBaseScore(TargetType target)
        {
            return target switch
            {
                TargetType.Perfect => perfectScore,
                TargetType.Backboard => backboardScore,
                _ => defaultScore
            };
        }
        
        private int GetBonusScore(TargetType target)
        {
            return target == TargetType.Backboard ? ActiveBonus : 0;
        }

        private int GetScoreMultiplier(PlayerNumber player)
        {
            return _enabledFireballs[player] ? 2 : 1;
        }

        private void AddStreak(PlayerNumber player)
        {
            if (_enabledFireballs[player]) return;
            
            _streaks[player]++;
        
            if (ShouldFireball(player))
            {
                _streaks[player] = 0;
                ActivateFireball(player);
            }
        
            _fireballUIController.AddStreak(player);
        }
        
        private void AddScore(PlayerNumber player, int score)
        {
            _scores[player] += Mathf.RoundToInt(score);
            _scoreUIController.UpdateScore(player, _scores[player]);
        }
        
        private void ResetStreaks()
        {
            ResetStreak(PlayerNumber.Player1);
            ResetStreak(PlayerNumber.Player2);
        }

        private void ResetStreak(PlayerNumber playerNumber)
        {
            _streaks[playerNumber] = 0;
            _enabledFireballs[playerNumber] = false;
            _fireballUIController.ResetStreak();
            DeactivateFireball(playerNumber);
        }
        
        private bool ShouldFireball(PlayerNumber player)
        {
            int streak = _streaks[player];
            return streak >= fireballStreakThreshold && !_enabledFireballs[player];
        }
        
        public int GetPlayerScore(PlayerNumber player)
        {
            return _scores.GetValueOrDefault(player, 0);
        }
    }
}