using Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.SkinSelector
{
    /// <summary>
    /// Handles the UI for ball skin and notifies changes to <see cref="PlayerDataManager"/>.
    /// Inherits navigation logic from <see cref="SkinSelector{T}"/>.
    /// </summary>
    public class BallSkinSelector : SkinSelector<BallSkinData>
    {
        [Header("UI Elements")]
        [SerializeField, Tooltip("Ball icon")] private Image ballImage;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(ballImage, $"{nameof(Image)} reference is missing on '{gameObject.name}'");
            #endif
            
            OnSelectionChanged += UpdateBallSkin;
        }

        private void UpdateBallSkin(BallSkinData ballSkinData)
        {
            
            #if UNITY_EDITOR
            Assert.IsNotNull(PlayerDataManager.Instance, $"{nameof(PlayerDataManager)} is missing");
            #endif
            
            ballImage.sprite = ballSkinData.icon;
            PlayerDataManager.Instance.SetBallSkin(ballSkinData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSelectionChanged -= UpdateBallSkin;
        }
    }
}