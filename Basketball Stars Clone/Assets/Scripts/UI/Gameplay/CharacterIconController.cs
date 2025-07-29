using Data;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Gameplay
{
    /// <summary>
    /// Controller that manages the player icons UI
    /// </summary>
    public class CharacterIconController : MonoBehaviour
    {
        [SerializeField] private PlayerNumber playerNumber;
        [SerializeField] private Image iconImage;
        
        private IPlayerDataService _playerDataService;
        private IGameDataService _gameDataService;

        private void Awake()
        {
            _playerDataService = ServiceLocator.PlayerDataService;
            _gameDataService = ServiceLocator.GameDataService;
            _playerDataService.OnCharacterSkinChanged += OnIconChanged;
            
            #if UNITY_EDITOR
            Assert.IsNotNull(iconImage, $"{nameof(iconImage)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            if (_playerDataService == null) return;
            
            RefreshIcon(_playerDataService.CurrentCharacterIcon(playerNumber));
        }

        private void OnDisable()
        {
            if (_playerDataService == null) return;
            _playerDataService.OnCharacterSkinChanged -= OnIconChanged;
        }

        private void OnDestroy()
        {
            if (_playerDataService == null) return;
            _playerDataService.OnCharacterSkinChanged -= OnIconChanged;
        }

        private void OnIconChanged(PlayerNumber player, CharacterSkinData character)
        {
            if (player != playerNumber) return;
            RefreshIcon(character.Icon);
        }

        private void RefreshIcon(Sprite icon)
        {
            iconImage.sprite = icon;
            
            bool hideSecondPlayerIcon = playerNumber == PlayerNumber.Player2 
                              && _gameDataService.CurrentGameMode == GameMode.Single;

            if (gameObject.activeSelf)
            {
                gameObject.SetActive(!hideSecondPlayerIcon);
            }
        }
    }
}