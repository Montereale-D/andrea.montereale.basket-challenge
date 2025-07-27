using System;
using Attributes;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Singleton responsible for managing player's choices (e.g. skins).
    /// Provides events to notify listeners when a change happens.
    /// </summary>
    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerDataManager Instance { get; private set; }
        
        [Header("Default Player Skin")]
        [SerializeField] private BallSkinData defaultBallSkin;
        [SerializeField] private CharacterSkinData defaultCharacterSkin;
        
        [Header("Selected Player Skin")]
        [ReadOnly] public BallSkinData selectedBallSkin;
        [ReadOnly] public CharacterSkinData selectedCharacterSkin;
        
        public event Action<BallSkinData> OnBallSkinChanged;
        public event Action<CharacterSkinData> OnCharacterSkinChanged;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            selectedBallSkin = defaultBallSkin;
            selectedCharacterSkin = defaultCharacterSkin;
        }

        public void SetBallSkin(BallSkinData newSkin)
        {
            if (newSkin is null || newSkin == selectedBallSkin)
                return;

            selectedBallSkin = newSkin;
            OnBallSkinChanged?.Invoke(newSkin);
        }

        public void SetCharacterSkin(CharacterSkinData newSkin)
        {
            if (newSkin is null || newSkin == selectedCharacterSkin)
                return;

            selectedCharacterSkin = newSkin;
            OnCharacterSkinChanged?.Invoke(newSkin);
        }

        private void OnDestroy()
        {
            if (Instance && Instance == this)
            {
                Instance = null;
            }
        }
    }
}