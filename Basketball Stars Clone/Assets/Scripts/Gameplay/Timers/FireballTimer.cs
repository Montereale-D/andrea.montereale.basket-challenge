using Enums;
using Events;
using UnityEngine;
using Utils;

namespace Gameplay.Timers
{
    /// <summary>
    /// Controls the countdown timer for the Fireball mode of a specific player.
    /// Handles pause, resume, and forced stop via game events, and notifies when the timer expires.
    /// </summary>
    public class FireballTimer : MonoBehaviour
    {
        [SerializeField] private PlayerNumber player;
        private TimerModel _timer;
        
        private void OnEnable()
        {
            _timer = new TimerModel();
            _timer.OnTimerEnd += OnFireballExpired;
            
            EventBus.Subscribe<FireballStartEvent>(StartFireball);
            EventBus.Subscribe<GamePauseEvent>(PauseTimer);
            EventBus.Subscribe<GameResumeEvent>(ResumeTimer);
            EventBus.Subscribe<GameEndEvent>(StopTimer);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<FireballStartEvent>(StartFireball);
            EventBus.Unsubscribe<GamePauseEvent>(PauseTimer);
            EventBus.Unsubscribe<GameResumeEvent>(ResumeTimer);
            EventBus.Unsubscribe<GameEndEvent>(StopTimer);
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
        }
        
        private void StartFireball(FireballStartEvent args)
        {
            if (args.Player != player) return;
            
            _timer.Initialize(args.Duration);
            _timer.Start();
        }
        
        private void PauseTimer()
        {
            _timer.Pause();
        }

        private void ResumeTimer()
        {
            _timer.Resume();
        }

        private void StopTimer()
        {
            _timer.Stop();
        }
    
        private void OnFireballExpired()
        {
            EventBus.Raise(new FireballExpiredEvent(player));
        }
    }
}