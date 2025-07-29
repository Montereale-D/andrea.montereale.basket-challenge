using System;
using Data;
using Enums;
using Interfaces;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Singleton responsible for managing player's choices (e.g. skins).
    /// Provides events to notify listeners when a change happens.
    /// </summary>
    public class PlayerDataManager : MonoBehaviour, IPlayerDataService
    {
        private static PlayerDataManager Instance { get; set; }
        
        [Header("Default Player Skin")]
        [SerializeField] private BallSkinData defaultBallSkin;
        [SerializeField] private CharacterSkinData defaultCharacterSkin;
        
        [Header("Default Enemy Skin")]
        [SerializeField] private BallSkinData defaultEnemyBallSkin;
        [SerializeField] private CharacterSkinData defaultEnemyCharacterSkin;
        
        public event Action<PlayerNumber, CharacterSkinData> OnCharacterSkinChanged;
        public event Action<PlayerNumber, BallSkinData> OnBallSkinChanged;

        private BallSkinData _selectedBallSkin;
        private CharacterSkinData _selectedCharacterSkin;
        private BallSkinData _enemyBallSkin;
        private CharacterSkinData _enemyCharacterSkin;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            ServiceLocator.PlayerDataService = this;
            
            _selectedBallSkin = defaultBallSkin;
            _selectedCharacterSkin = defaultCharacterSkin;
            _enemyBallSkin = defaultEnemyBallSkin;
            _enemyCharacterSkin = defaultEnemyCharacterSkin;
        }
        
        private void OnDestroy()
        {
            if (Instance && Instance == this)
            {
                Instance = null;
                
                ServiceLocator.PlayerDataService = null;
            }
        }

        public void SetBallSkin(PlayerNumber player, BallSkinData newSkin)
        {
            if (newSkin is null) return;
            
            if (player == PlayerNumber.Player1)
            {
                _selectedBallSkin = newSkin;
            }
            else
            {
                _enemyBallSkin = newSkin;
            }
            
            OnBallSkinChanged?.Invoke(player, newSkin);
        }

        public void SetCharacterSkin(PlayerNumber player, CharacterSkinData newSkin)
        {
            if (newSkin is null) return;

            if (player == PlayerNumber.Player1)
            {
                _selectedCharacterSkin = newSkin;
            }
            else
            {
                _enemyCharacterSkin = newSkin;
            }
            
            OnCharacterSkinChanged?.Invoke(player, newSkin);
        }
        
        public Sprite CurrentCharacterIcon(PlayerNumber player)
        {
            return player == PlayerNumber.Player1
                ? _selectedCharacterSkin.Icon
                : _enemyCharacterSkin.Icon;
        }
        
        public Material GetBallMaterial(PlayerNumber player)
        {
            var skinData = player == PlayerNumber.Player1 ? _selectedBallSkin : _enemyBallSkin;
            return skinData.ModelPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }

        public GameObject GetCharacterPrefab(PlayerNumber player)
        {
            return player == PlayerNumber.Player1
                ? _selectedCharacterSkin.ModelPrefab
                : _enemyCharacterSkin.ModelPrefab;
        }

        public CharacterSkinData GetCharacterSkinData(PlayerNumber player)
        {
            return player == PlayerNumber.Player1 
                ? _selectedCharacterSkin 
                : _enemyCharacterSkin;
        }
        public BallSkinData GetBallSkinData(PlayerNumber player)
        {
            return player == PlayerNumber.Player1 
                ? _selectedBallSkin 
                : _enemyBallSkin;
        }
    }
}