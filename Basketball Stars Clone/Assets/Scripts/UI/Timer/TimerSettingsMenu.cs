using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Timer
{
    /// <summary>
    /// Provides UI controls for adjusting a timer setting within specified limits.
    /// </summary>
    public class TimerSettingsMenu : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private Text timerText;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;

        [Header("Time options")]
        [Tooltip("Minimum allowed time (in seconds)")] 
        [SerializeField] private int minTime = 30;

        [Tooltip("Maximum allowed time (in seconds)")] 
        [SerializeField] private int maxTime = 120;

        [Tooltip("Step to increase or decrease time (in seconds)")] 
        [SerializeField] private int timeStep = 10;

        // current and displayed time in seconds
        private int _currentTime = 60;

        private void Start()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(timerText, $"{nameof(Text)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(increaseButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(decreaseButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            #endif
            
            UpdateTimerText();
            UpdateButtonStates();

            increaseButton.onClick.AddListener(IncreaseTime);
            decreaseButton.onClick.AddListener(DecreaseTime);
        }

        private void IncreaseTime()
        {
            if (_currentTime + timeStep > maxTime) return;

            _currentTime += timeStep;
            UpdateTimerText();
            UpdateButtonStates();
        }

        private void DecreaseTime()
        {
            if (_currentTime - timeStep < minTime) return;

            _currentTime -= timeStep;
            UpdateTimerText();
            UpdateButtonStates();
        }

        private void UpdateTimerText()
        {
            int minutes = _currentTime / 60;
            int seconds = _currentTime % 60;
            timerText.text = $"{minutes} : {(seconds < 10 ? "0" : "")}{seconds}";
        }

        private void UpdateButtonStates()
        {
            increaseButton.interactable = _currentTime + timeStep <= maxTime;
            decreaseButton.interactable = _currentTime - timeStep >= minTime;
        }

        private void OnDestroy()
        {
            increaseButton.onClick.RemoveListener(IncreaseTime);
            decreaseButton.onClick.RemoveListener(DecreaseTime);
        }
    }
}