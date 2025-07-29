using Events;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Gameplay.Timers
{
    /// <summary>
    /// Manages the main game countdown timer.
    /// It's handling start, pause, resume and end events.
    /// Updates the UI accordingly and raises a TimeEndEvent when the timer reaches zero.
    /// </summary>
    public class GameTimer : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI timerText;

        private TimerModel _timer;
        private IGameDataService _dataService;

        private void Awake()
        {
            _timer = new TimerModel();
            
            #if UNITY_EDITOR
            Assert.IsNotNull(timerText, $"{nameof(TextMeshProUGUI)} reference is missing on '{gameObject.name}'");
            #endif
        }
        private void OnEnable()
        {
            _dataService = ServiceLocator.GameDataService;
        
            EventBus.Subscribe<GameStartEvent>(OnGameStart);
            EventBus.Subscribe<GamePauseEvent>(OnGamePause);
            EventBus.Subscribe<GameResumeEvent>(OnGameResume);

            _timer.OnTimerEnd += HandleTimerEnd;
        }
        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartEvent>(OnGameStart);
            EventBus.Unsubscribe<GamePauseEvent>(OnGamePause);
            EventBus.Unsubscribe<GameResumeEvent>(OnGameResume);
        
            _timer.OnTimerEnd -= HandleTimerEnd;
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
            UpdateTimerDisplay();
        }
    
        private void OnGameStart(GameStartEvent obj)
        {
            float durationSec = _dataService?.GameTimerDuration ?? 0f;
        
            _timer.Initialize(durationSec);
            _timer.Start();
            UpdateTimerDisplay();
        }
    
        private void OnGamePause(GamePauseEvent obj)
        {
            _timer.Pause();
        }
    
        private void OnGameResume(GameResumeEvent obj)
        {
            _timer.Resume();
        }

        private void HandleTimerEnd()
        {
            EventBus.Raise(new TimeEndEvent());
        }
    
        private void UpdateTimerDisplay()
        {
            if (!timerText) return;

            if (_timer.IsRunning || _timer.IsPaused)
            {
                int minutes = Mathf.FloorToInt(_timer.Remaining / 60f);
                int seconds = Mathf.FloorToInt(_timer.Remaining % 60f);
                timerText.text = $"{minutes:0}:{seconds:00}";
            }
            else
            {
                timerText.text = string.Empty;
            }
        }
    }
}