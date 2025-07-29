namespace Utils
{
    using System;

    /// <summary>
    /// Class that manages a timer.
    /// Tracks duration, remaining time and state transitions.
    /// </summary>
    public class TimerModel
    {
        private float _duration;
        private float _remaining;
        private bool _isRunning;
        private bool _isPaused;

        public float Remaining => _remaining;
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;

        public event Action OnTimerEnd;

        public void Initialize(float seconds)
        {
            _duration = seconds;
            _remaining = seconds;
            _isRunning = false;
            _isPaused = false;
        }

        public void Start()
        {
            if (_duration <= 0f) return;
            _remaining = _duration;
            _isRunning = true;
            _isPaused = false;
        }

        public void Pause()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _isPaused = true;
            }
        }

        public void Resume()
        {
            if (_isPaused)
            {
                _isRunning = true;
                _isPaused = false;
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _remaining = 0f;
        }
        
        public void Tick(float deltaTime)
        {
            if (!_isRunning) return;

            _remaining -= deltaTime;
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                _isRunning = false;
                OnTimerEnd?.Invoke();
            }
        }
    }
}