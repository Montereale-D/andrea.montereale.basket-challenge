using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using AudioType = Interfaces.AudioType;

namespace UI.Menu.Settings
{
    /// <summary>
    /// Controller class to set up the correct audio volume
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private AudioType audioType;
    
        private Slider _slider;
        private float _lastValue;
        private ISoundService _soundService;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            _soundService = ServiceLocator.SoundService;
            
            _slider.value = _soundService.GetVolume(audioType);
            _slider.onValueChanged.AddListener(ChangeVolume);
            _lastValue = _slider.value;
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(ChangeVolume);
        }

        private void ChangeVolume(float volume)
        {
            _soundService.SetVolume(audioType, volume);

            if (Mathf.Abs(_lastValue - volume) >= 0.1f)
            {
                _lastValue = volume;
                _soundService.PlaySound(SoundType.UI_SELECT);
            }
        }
    }
}