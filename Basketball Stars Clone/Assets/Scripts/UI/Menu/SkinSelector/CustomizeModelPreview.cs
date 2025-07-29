using System.Collections.Generic;
using Data;
using Enums;
using Gameplay.Basketball;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Menu.SkinSelector
{
    /// <summary>
    ///A dedicated MonoBehaviour for previewing character and ball models inside the UI.
    /// 
    /// Note: The preload strategy loads all models at once, which is fine for a small number of skins.
    /// For a large number of models consider another approach.
    /// </summary>
    public class CustomizeModelPreview : MonoBehaviour
    {
        [Header("Ball Preview")]
        [SerializeField] private GameObject ballContainer;
        [SerializeField] private GameObject basketballPrefab;
        
        [Header("Character Preview")]
        [SerializeField] private GameObject characterContainer;
        [SerializeField] private List<CharacterSkinData> characterSkins;
        
        private readonly Dictionary<CharacterSkinType, GameObject> _characterGameObjects = new();

        private IPlayerDataService _dataService;
        private BasketBall _basketball;
        private GameObject _character;
        
        private void Start()
        {
            
            #if UNITY_EDITOR
            Assert.IsNotNull(ballContainer, $"Ball container reference is missing on '{gameObject.name}'");
            Assert.IsTrue(basketballPrefab, $"Ball reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(characterContainer, $"Character container reference is missing on '{gameObject.name}'");
            Assert.IsTrue(characterSkins.Count > 0, $"Character skins reference is missing on '{gameObject.name}'");
            #endif
            
            CharactersPreload();
            
            _dataService = ServiceLocator.PlayerDataService;
            _dataService.OnCharacterSkinChanged += UpdateCharacterSkin;
            _dataService.OnBallSkinChanged += UpdateBallSkin;
            
            GameObject ball = Instantiate(basketballPrefab, ballContainer.transform);
            _basketball = ball.GetComponent<BasketBall>();
            
            UpdateBallSkin(PlayerNumber.Player1, _dataService.GetBallSkinData(PlayerNumber.Player1));
            UpdateCharacterSkin(PlayerNumber.Player1, _dataService.GetCharacterSkinData(PlayerNumber.Player1));
        }

        private void CharactersPreload()
        {
            foreach (var characterSkin in characterSkins)
            {
                GameObject character = Instantiate(characterSkin.ModelPrefab, characterContainer.transform);
                character.SetActive(false);
                _characterGameObjects.Add(characterSkin.SkinType, character);
            }
        }

        private void UpdateBallSkin(PlayerNumber player, BallSkinData skinData)
        {
            if (player != PlayerNumber.Player1) return;
            
            Material material = _dataService.GetBallMaterial(PlayerNumber.Player1);
            _basketball.SetMeshMaterial(material);
        }

        private void UpdateCharacterSkin(PlayerNumber player, CharacterSkinData skinData)
        {
            if (player != PlayerNumber.Player1) return;
            
            if (_character)
            {
                _character.SetActive(false);
            }

            if (_characterGameObjects.TryGetValue(skinData.SkinType, out var newCharacter))
            {
                newCharacter.SetActive(true);
                _character = newCharacter;
            }
            else
            {
                Debug.LogError("Character preview error: skin data not found");
            }
        }
        
        private void OnDestroy()
        {
            _dataService.OnCharacterSkinChanged -= UpdateCharacterSkin;
            _dataService.OnBallSkinChanged -= UpdateBallSkin;
            _dataService = null;
        }
    }
}