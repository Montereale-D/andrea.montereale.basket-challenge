using Enums;
using Interfaces;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Singleton responsible for managing game mode settings.
    /// </summary>
    public class GameDataManager : MonoBehaviour, IGameDataService
    {
        public float GameTimerDuration { get; private set; } = 60;
        public GameMode CurrentGameMode { get; private set; } = GameMode.Single;
        public AiDifficulty AIDifficulty { get; private set; } = AiDifficulty.Easy;

        private static GameDataManager Instance { get; set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            ServiceLocator.GameDataService = this;
        }
        
        public void SetTimerDuration(int newTimerDuration)
        {
            GameTimerDuration = newTimerDuration;
        }

        public void SetGameMode(GameMode mode)
        {
            CurrentGameMode = mode;
        }

        public void SetAIDifficulty(AiDifficulty difficulty)
        {
            AIDifficulty = difficulty;
        }
    }
}