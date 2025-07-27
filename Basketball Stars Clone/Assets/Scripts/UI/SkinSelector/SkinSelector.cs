using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.SkinSelector
{
    /// <summary>
    /// Generic base class for creating UI skin selectors.
    /// Manages a list of items and notify listeners when the selection changes.
    /// Intended to be extended by specific skin types (e.g. ball, character).
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    public class SkinSelector<T> : MonoBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        
        [Header("Skins")]
        [SerializeField] private List<T> items = new();
        
        protected Action<T> OnSelectionChanged;
        
        private int _currentIndex;

        protected virtual void Start()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(previousButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(nextButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            #endif
            
            previousButton.onClick.AddListener(SelectPrevious);
            nextButton.onClick.AddListener(SelectNext);
            UpdateUI();
        }

        private void SelectPrevious()
        {
            if (_currentIndex <= 0) return;

            _currentIndex--;
            UpdateUI();
        }

        private void SelectNext()
        {
            if (_currentIndex >= items.Count - 1) return;

            _currentIndex++;
            UpdateUI();
        }

        private void UpdateUI()
        {
            UpdateButtonsStatus();
            OnSelectionChanged?.Invoke(items[_currentIndex]);
        }

        private void UpdateButtonsStatus()
        {
            previousButton.interactable = _currentIndex > 0;
            nextButton.interactable = _currentIndex < items.Count - 1;
        }

        protected virtual void OnDestroy()
        {
            previousButton.onClick.RemoveListener(SelectPrevious);
            nextButton.onClick.RemoveListener(SelectNext);
        }
    }
}