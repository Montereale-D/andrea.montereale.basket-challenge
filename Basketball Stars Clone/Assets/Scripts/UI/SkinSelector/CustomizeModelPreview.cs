using System.Collections.Generic;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.SkinSelector
{
    /// <summary>
    /// Manages the model previews for the player customization.
    /// Preloads all available skin models and activates the selected ones based on player data.
    /// Listens for skin change events from <see cref="PlayerDataManager"/> to update the previews dynamically.
    ///
    /// Note: The preload strategy loads all models at once, which is fine for a small number of skins.
    /// For a large number of models, consider another approach.
    /// </summary>
    public class CustomizeModelPreview : MonoBehaviour
    {
        [Header("Ball Preview")]
        [SerializeField] private GameObject ballContainer;
        [SerializeField] private List<BallSkinData> ballSkins;
        
        [Header("Character Preview")]
        [SerializeField] private GameObject characterContainer;
        [SerializeField] private List<CharacterSkinData> characterSkins;

        private readonly Dictionary<BallSkinType, GameObject> _ballGameObjects = new();
        private readonly Dictionary<CharacterSkinType, GameObject> _characterGameObjects = new();
        
        private GameObject _selectedBall;
        private GameObject _selectedCharacter;

        private void Start()
        {
            
            #if UNITY_EDITOR
            Assert.IsNotNull(ballContainer, $"Ball container reference is missing on '{gameObject.name}'");
            Assert.IsTrue(ballSkins.Count > 0, $"Ball skins reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(characterContainer, $"Character container reference is missing on '{gameObject.name}'");
            Assert.IsTrue(characterSkins.Count > 0, $"Character skins reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(PlayerDataManager.Instance, $"{typeof(PlayerDataManager)} is missing");
            #endif
            
            BallsPreload();
            CharactersPreload();
            
            UpdateBallSkin(PlayerDataManager.Instance?.selectedBallSkin);
            UpdateCharacterSkin(PlayerDataManager.Instance?.selectedCharacterSkin);

            if (!PlayerDataManager.Instance) return;
            
            PlayerDataManager.Instance.OnBallSkinChanged += UpdateBallSkin;
            PlayerDataManager.Instance.OnCharacterSkinChanged += UpdateCharacterSkin;
        }

        private void BallsPreload()
        {
            foreach (var ballSkin in ballSkins)
            {
                GameObject ball = Instantiate(ballSkin.modelPrefab, ballContainer.transform);
                ball.SetActive(false);
                _ballGameObjects.Add(ballSkin.skinType, ball);
            }
        }
        
        private void CharactersPreload()
        {
            foreach (var characterSkin in characterSkins)
            {
                GameObject character = Instantiate(characterSkin.modelPrefab, characterContainer.transform);
                character.SetActive(false);
                _characterGameObjects.Add(characterSkin.skinType, character);
            }
        }

        private void UpdateBallSkin(BallSkinData selectedSkin)
        {
            if (_selectedBall)
            {
                _selectedBall.SetActive(false);
            }

            if (_ballGameObjects.TryGetValue(selectedSkin.skinType, out GameObject newBall))
            {
                newBall.SetActive(true);
                _selectedBall = newBall;
            }
        }

        private void UpdateCharacterSkin(CharacterSkinData selectedSkin)
        {
            if (_selectedCharacter)
            {
                _selectedCharacter.SetActive(false);
            }

            if (_characterGameObjects.TryGetValue(selectedSkin.skinType, out var newCharacter))
            {
                newCharacter.SetActive(true);
                _selectedCharacter = newCharacter;
            }
        }
        
        private void OnDestroy()
        {
            if (!PlayerDataManager.Instance) return;
            
            PlayerDataManager.Instance.OnBallSkinChanged -= UpdateBallSkin;
            PlayerDataManager.Instance.OnCharacterSkinChanged -= UpdateCharacterSkin;
        }
    }
}