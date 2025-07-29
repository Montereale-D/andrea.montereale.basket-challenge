using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utils;

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
        [SerializeField] private Button confirmButton;

        [Header("Time options")]
        [SerializeField, Tooltip("Minimum allowed time (in seconds)")] private int minTime = 30;
        [SerializeField, Tooltip("Maximum allowed time (in seconds)")] private int maxTime = 120;
        [SerializeField, Tooltip("Step to increase or decrease time (in seconds)")] private int timeStep = 10;

        private TimerUIModel _timer;
        private IGameDataService _dataService;
        private ISoundService _soundService;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(timerText, $"{nameof(Text)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(increaseButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(decreaseButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(confirmButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            #endif
            
            _dataService = ServiceLocator.GameDataService;
            _soundService = ServiceLocator.SoundService;
            
            int defaultTime = _dataService != null ? (int)_dataService.GameTimerDuration : maxTime;
            _timer = new TimerUIModel(defaultTime, minTime, maxTime, timeStep);
            
            _timer.OnValueChanged += OnTimerValueChanged;
            increaseButton.onClick.AddListener(_timer.Increase);
            decreaseButton.onClick.AddListener(_timer.Decrease);
            confirmButton.onClick.AddListener(OnConfirm);
        }

        private void Start()
        {
            OnTimerValueChanged(_timer.Current);
        }

        private void OnTimerValueChanged(int newSeconds)
        {
            int minutes = newSeconds / 60;
            int seconds = newSeconds % 60;
            timerText.text = $"{minutes}:{seconds:00}";
            
            increaseButton.interactable = newSeconds + timeStep <= maxTime;
            decreaseButton.interactable = newSeconds - timeStep >= minTime;
            
            _soundService.PlaySound(SoundType.UI_SELECT);
        }

        private void OnConfirm()
        {
            if (_dataService == null)
            {
                Debug.LogError("PlayerDataService not available");
                return;
            }
            
            _dataService.SetTimerDuration(_timer.Current);
            _soundService.PlaySound(SoundType.UI_CONFIRM);
        }

        private void OnDestroy()
        {
            if (_timer != null)
            {
                _timer.OnValueChanged -= OnTimerValueChanged;
                increaseButton.onClick.RemoveListener(_timer.Increase);
                decreaseButton.onClick.RemoveListener(_timer.Decrease);
            }
            
            confirmButton.onClick.RemoveListener(OnConfirm);
        }
    }
}