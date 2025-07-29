using System;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// A helper class that manages a collection of items of type T.
    /// It tracks the current index, exposes whether you can move next or previous and raises an event
    /// whenever the selection changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListNavigator<T>
    {
        private readonly IList<T> _items;
        private int _currentIndex;

        public ListNavigator(IList<T> items, int startIndex = 0)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Items cannot be null or empty.", nameof(items));
            if (startIndex < 0 || startIndex >= items.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            _items = items;
            _currentIndex = startIndex;
        }
        
        public T Current => _items[_currentIndex];
        public bool CanMovePrevious => _currentIndex > 0;
        public bool CanMoveNext => _currentIndex < _items.Count - 1;
        
        public int CurrentIndex => _currentIndex;
        public int Count => _items.Count;
        
        public event Action<T> OnValueChanged;

        public void MovePrevious()
        {
            if (!CanMovePrevious) return;
            _currentIndex--;
            OnValueChanged?.Invoke(Current);
        }

        public void MoveNext()
        {
            if (!CanMoveNext) return;
            _currentIndex++;
            OnValueChanged?.Invoke(Current);
        }
        
        public void SetIndex(int index)
        {
            if (index < 0 || index >= _items.Count) 
                throw new ArgumentOutOfRangeException(nameof(index));
            _currentIndex = index;
            OnValueChanged?.Invoke(Current);
        }

        public void Clear()
        {
            OnValueChanged = null;
            _items.Clear();
        }
    }
}