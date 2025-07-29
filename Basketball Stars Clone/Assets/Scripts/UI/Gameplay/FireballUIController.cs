using Enums;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Gameplay
{
    /// <summary>
    /// Controller class that manages the fireball UI
    /// </summary>
    public class FireballUIController : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private GameObject fireballUIPanel;
        [SerializeField] private GameObject activeFireballUI;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(slider, $"{nameof(Slider)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(fireballUIPanel, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(activeFireballUI, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void Start()
        {
            slider.value = 0;
            activeFireballUI.SetActive(false);
        }

        public void SetThreshold(int fireballStreakThreshold)
        {
            slider.maxValue = fireballStreakThreshold;
            slider.value = 0;
        }

        public void ActivateFireball(PlayerNumber player)
        {
            if (player != PlayerNumber.Player1) return;
            
            activeFireballUI.gameObject.SetActive(true);
        }
        
        public void DeactivateFireball(PlayerNumber player)
        {
            if (player != PlayerNumber.Player1) return;
            
            ResetStreak();
        }

        public void ResetStreak()
        {
            slider.value = 0;
            activeFireballUI.gameObject.SetActive(false);
        }

        public void AddStreak(PlayerNumber player)
        {
            if (player != PlayerNumber.Player1) return;

            slider.value++;
        }

        public void Show()
        {
            fireballUIPanel.gameObject.SetActive(true);
        }

        public void Hide()
        {
            fireballUIPanel.gameObject.SetActive(false);
        }
    }
}