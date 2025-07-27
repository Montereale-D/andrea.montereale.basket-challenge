using Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.SkinSelector
{
    /// <summary>
    /// Handles the UI for character skin and notifies changes to <see cref="PlayerDataManager"/>.
    /// Inherits navigation logic from <see cref="SkinSelector{T}"/>.
    /// </summary>
    public class CharacterSkinSelector : SkinSelector<CharacterSkinData>
    {
        [Header("UI Elements")]
        [SerializeField, Tooltip("Character icon")] private Image characterImage;

        private void Awake()
        {
            
            #if UNITY_EDITOR
            Assert.IsNotNull(characterImage, $"{nameof(Image)} reference is missing on '{gameObject.name}'");
            #endif
            
            OnSelectionChanged += UpdateCharacterSkin;
        }

        private void UpdateCharacterSkin(CharacterSkinData characterSkinData)
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(PlayerDataManager.Instance, $"{nameof(PlayerDataManager)} is missing");
            #endif
            
            characterImage.sprite = characterSkinData.icon;
            PlayerDataManager.Instance.SetCharacterSkin(characterSkinData);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSelectionChanged -= UpdateCharacterSkin;
        }
    }
}