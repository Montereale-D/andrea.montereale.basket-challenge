using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utils;

namespace UI.Menu.SkinSelector
{
    /// <summary>
    /// A MonoBehaviour that binds UI buttons and an image display a selected skin.
    /// It uses button clicks for the navigation, updates button interactability based on available items
    /// and automatically refreshes the icon whenever the navigator’s value changes.
    /// Subclasses hook into its protected selection-changed event to apply domain-specific behavior.
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    public class SkinSelectorView<T> : MonoBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        
        [Header("Icon Display")]
        [SerializeField, Tooltip("Icon image to display the current selection")] 
        protected Image iconImage;
        
        [Header("Skins")]
        [SerializeField] protected List<T> items = new();
        
        protected Action<T> OnSelectionChanged;
        
        protected ListNavigator<T> Navigator;

        protected Func<T, Sprite> GetSpriteForItem;
        
        protected virtual void Start()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(previousButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(nextButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(iconImage, $"{nameof(Image)} reference is missing on '{gameObject.name}'");
            #endif

            if (items == null || items.Count == 0)
            {
                previousButton.interactable = false;
                nextButton.interactable = false;
                return;
            }
            
            Navigator = new ListNavigator<T>(items, GetStartIndex());
            previousButton.onClick.AddListener(Navigator.MovePrevious);
            nextButton.onClick.AddListener(Navigator.MoveNext);

            HandleValueChanged(Navigator.Current);
            Navigator.OnValueChanged += HandleValueChanged;
        }

        private void HandleValueChanged(T newValue)
        {
            previousButton.interactable = Navigator.CanMovePrevious;
            nextButton.interactable = Navigator.CanMoveNext;
            
            OnSelectionChanged?.Invoke(newValue);
            
            if (GetSpriteForItem != null && iconImage)
            {
                iconImage.sprite = GetSpriteForItem(newValue);
            }
        }

        protected virtual int GetStartIndex()
        {
            return 0;
        }

        protected virtual void OnDestroy()
        {
            previousButton.onClick.RemoveListener(Navigator.MovePrevious);
            nextButton.onClick.RemoveListener(Navigator.MoveNext);
            
            if (Navigator != null)
                Navigator.OnValueChanged -= HandleValueChanged;

            Navigator.Clear();
        }
    }
}