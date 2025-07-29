using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Menu.Settings
{
    /// <summary>
    /// Controller class to set up the correct game mode from UI
    /// </summary>
    public class GameSettingsUI : MonoBehaviour
    {
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button easyAIButton;
        [SerializeField] private Button normalAIButton;
        [SerializeField] private Button hardAIButton;

        private IGameDataService _dataService;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(trainingButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(easyAIButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(normalAIButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(hardAIButton, $"{nameof(Button)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            _dataService = ServiceLocator.GameDataService;
            trainingButton.onClick.AddListener(SetTrainingMode);
            easyAIButton.onClick.AddListener(()=> SetAiMode(AiDifficulty.Easy));
            normalAIButton.onClick.AddListener(()=> SetAiMode(AiDifficulty.Normal));
            hardAIButton.onClick.AddListener(()=> SetAiMode(AiDifficulty.Hard));
        }

        private void OnDisable()
        {
            _dataService = null;
            trainingButton.onClick.RemoveListener(SetTrainingMode);
        }

        private void SetTrainingMode()
        {
            _dataService.SetGameMode(GameMode.Single);
        }
        
        private void SetAiMode(AiDifficulty ai)
        {
            _dataService.SetGameMode(GameMode.AI);
            _dataService.SetAIDifficulty(ai);
        }
    }
}