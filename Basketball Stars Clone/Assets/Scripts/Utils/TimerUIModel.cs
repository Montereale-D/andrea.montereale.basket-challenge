using System;

namespace Utils
{
    /// <summary>
    /// Class that manages a timer value within defined bounds.
    /// Raises an event whenever the value changes.
    /// </summary>
    public class TimerUIModel
    {
        public int Min { get; }
        public int Max { get; }
        public int Step { get; }

        private int _current;
        public int Current
        {
            get => _current;
            private set
            {
                if (_current == value) return;
                _current = value;
                OnValueChanged?.Invoke(_current);
            }
        }
        
        public event Action<int> OnValueChanged;

        public TimerUIModel(int initial, int min, int max, int step)
        {
            if (min > max) throw new ArgumentException("min must be <= max");
            if (step <= 0) throw new ArgumentException("step must be > 0");
            Min = min;
            Max = max;
            Step = step;
            _current = Math.Clamp(initial, min, max);
        }

        public void Increase() => Set(_current + Step);

        public void Decrease() => Set(_current - Step);

        public void Set(int value) => Current = Math.Clamp(value, Min, Max);
    }

}