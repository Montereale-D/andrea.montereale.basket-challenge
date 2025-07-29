using Attributes;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Gameplay.Player.AI
{
    /// <summary>
    /// AI-controlled player that extends default player functionality.
    /// Selects difficulty settings based on game mode and difficulty level.
    /// Uses timers and random chances to simulate decision-making for throwing.
    /// </summary>
    public class PlayerAI : Player.Human.Player
    {
        [ReadOnly] public AiSettings currentSettings;
        [SerializeField] private AiSettings easySettings;
        [SerializeField] private AiSettings normalSettings;
        [SerializeField] private AiSettings hardSettings;

        private float _timer;
        private bool _isRunning;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(easySettings);
            Assert.IsNotNull(normalSettings);
            Assert.IsNotNull(hardSettings);
            #endif
        }

        protected override void Start()
        {
            var gameDataService = ServiceLocator.GameDataService;
            
            if (gameDataService.CurrentGameMode == GameMode.Single)
            {
                gameObject.SetActive(false);
                return;
            }
            
            currentSettings = gameDataService.AIDifficulty switch
            {
                AiDifficulty.Easy => easySettings,
                AiDifficulty.Hard => hardSettings,
                _ => normalSettings
            };
            
            base.Start();
            _timer = currentSettings.reactionTime;
            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning) return;

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            if (currentSettings.reactionToBonus && ScoreDataService.ActiveBonus > 0)
            {
                TriggerRandomBackboard();
            }
            else
            {
                TriggerRandomEvent();
            }
                
            _isRunning = false;
        }

        private void TriggerRandomBackboard()
        {
            float r = Random.value;
            float cumulative = 0f;

            cumulative += currentSettings.backboardChance + currentSettings.perfectChance;
            if (r <= cumulative)
            {
                SwipeRequest(TargetType.Backboard);
                return;
            }

            cumulative += currentSettings.noBackboardChance + currentSettings.noPerfectChance;
            if (r <= cumulative)
            {
                SwipeRequest(TargetType.OverBackboard);
            }
        }
        
        private void TriggerRandomEvent()
        {
            float r = Random.value;
            float cumulative = 0f;

            cumulative += currentSettings.perfectChance;
            if (r <= cumulative)
            {
                SwipeRequest(TargetType.Perfect);
                return;
            }

            cumulative += currentSettings.backboardChance;
            if (r <= cumulative)
            {
                SwipeRequest(TargetType.Backboard);
                return;
            }

            cumulative += currentSettings.noPerfectChance;
            if (r <= cumulative)
            {
                SwipeRequest(TargetType.UnderPerfect);
                return;
            }

            cumulative += currentSettings.noBackboardChance;
            if (r <= cumulative)
            {
                SwipeRequest(TargetType.OverBackboard);
            }
        }

        protected override void HandleBallResult(bool success)
        {
            base.HandleBallResult(success);
            _timer = currentSettings.reactionTime;
            _isRunning = true;
        }
    }
}