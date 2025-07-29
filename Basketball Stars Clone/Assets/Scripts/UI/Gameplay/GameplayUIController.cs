using Enums;
using Events;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Gameplay
{
    /// <summary>
    /// Controller class that manages the gameplay UI
    /// </summary>
    [RequireComponent(typeof(RecapUIController))]
    public class GameplayUIController : MonoBehaviour
    {
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject score1Panel;
        [SerializeField] private GameObject score2Panel;

        private RecapUIController _recapUIController;
        private IGameDataService _dataService;

        private void Awake()
        {
            _recapUIController = GetComponent<RecapUIController>();
            
            #if UNITY_EDITOR
            Assert.IsNotNull(optionsPanel, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(score1Panel, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(score2Panel, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(_recapUIController, $"{nameof(RecapUIController)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            _dataService = ServiceLocator.GameDataService;
            EventBus.Subscribe<GameStartEvent>(OnGameStartEvent);
            EventBus.Subscribe<GameEndEvent>(OnGameEndEvent);
        }

        private void OnDisable()
        {
            _dataService = null;
            EventBus.Unsubscribe<GameStartEvent>(OnGameStartEvent);
            EventBus.Unsubscribe<GameEndEvent>(OnGameEndEvent);
        }
    
        private void OnGameStartEvent(GameStartEvent args)
        {
            _recapUIController.Hide();
            optionsPanel.SetActive(false);
        
            score1Panel.SetActive(true);
            score2Panel.SetActive(_dataService.CurrentGameMode == GameMode.AI);
        }

        private void OnGameEndEvent(GameEndEvent args)
        {
            if (_dataService.CurrentGameMode == GameMode.Single)
            {
                _recapUIController.ShowSoloRecap(args.Player1Score);
            }
            else
            {
                _recapUIController.ShowDuelRecap(args.Player1Score, args.Player2Score);
            }
            
            ServiceLocator.SoundService.PlaySound(SoundType.UI_RECAP);
        }
    }
}