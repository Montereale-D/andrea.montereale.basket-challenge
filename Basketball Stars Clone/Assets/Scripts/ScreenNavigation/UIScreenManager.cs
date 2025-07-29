using System.Collections.Generic;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ScreenNavigation
{
    /// <summary>
    /// Manages the navigation between UI screens using a stack-based system.
    /// Supports both Single and Additive screen management modes.
    /// 
    /// In Single mode, only one screen is active at a time.
    /// In Additive mode, multiple screens can be active, with back navigation deactivating the current screen.
    /// </summary>
    public class UIScreenManager : MonoBehaviour
    {
        private enum ScreenManagerMode
        {
            Single,
            Additive
        }

        private enum StackOperation
        {
            Push,
            Pop
        }
        
        public static UIScreenManager Instance { get; private set; }
        
        [SerializeField] private ScreenManagerMode mode;
        [FormerlySerializedAs("_screensReference")] [SerializeField] private List<ScreenData> screensReference;
        
        private GameObject _currentScreen;
        private readonly Dictionary<UIScreenID, GameObject> _screens = new();
        private readonly Stack<NavigationData> _navigationStack = new();

        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            
            #if UNITY_EDITOR
            Assert.IsTrue(screensReference.Count > 0, "Empty screens reference");
            #endif
            
            LoadScreens();
            NavigateToScreen(screensReference[0].ScreenID);
        }

        private void LoadScreens()
        {
            foreach (var screenData in screensReference)
            {
                _screens.Add(screenData.ScreenID, screenData.Screen);
                screenData.Screen.SetActive(false);
            }
        }

        public void NavigateToScreen(UIScreenID screenID, object data = null)
        {
            _navigationStack.Push(new NavigationData { ScreenID = screenID, Data = data });
            OnStackedChanged(StackOperation.Push);
        }

        public void NavigateBack()
        {
            if (_navigationStack.Count <= 1) return;
            
            _navigationStack.Pop();
            OnStackedChanged(StackOperation.Pop);
        }

        private void OnStackedChanged(StackOperation stackOperation)
        {
            if (_navigationStack.Count <= 0) return;
            
            UIScreenID screenID = _navigationStack.Peek().ScreenID;

            if (!_screens.TryGetValue(screenID, out var screen)) return;
            
            DeactivateScreen(stackOperation);
            ActivateScreen(screen);
        }

        private void DeactivateScreen(StackOperation stackOperation)
        {
            if (_currentScreen && ShouldDeactivateCurrent(stackOperation))
            {
                _currentScreen.SetActive(false);
            }
        }
        
        private void ActivateScreen(GameObject screen)
        {
            _currentScreen = screen;
            _currentScreen.SetActive(true);
        }

        private bool ShouldDeactivateCurrent(StackOperation stackOp)
        {
            return mode == ScreenManagerMode.Single ||
                   (mode == ScreenManagerMode.Additive && stackOp == StackOperation.Pop);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}