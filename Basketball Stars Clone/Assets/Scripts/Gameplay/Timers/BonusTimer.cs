using Enums;
using Events;
using Gameplay.Bonus;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;
using Random = UnityEngine.Random;

namespace Gameplay.Timers
{
    /// <summary>
    /// Manages the random timed appearance of score bonuses during the game.
    /// Triggers bonuses based on configured chances and cooldown intervals.
    /// Each bonus has a visual indicator (UI) and a limited duration, after which the effect ends.
    /// Bonus types include Common, Rare, and Very Rare, each with its own duration and spawn chance.
    /// </summary>
    public class BonusTimer : MonoBehaviour
    {
        [Header("Bonus UI")] 
        [SerializeField] private GameObject commonBonusUI;
        [SerializeField] private GameObject rareBonusUI;
        [SerializeField] private GameObject veryRareBonusUI;

        [Header("Bonus Settings")] 
        [SerializeField] private BonusSettings bonusSettings;

        private float _commonBonusDuration;
        private float _rareBonusDuration;
        private float _veryRareBonusDuration;

        private float _commonBonusChance;
        private float _rareBonusChance;
        private float _veryRareBonusChance;

        private float _bonusCooldown;

        private TimerModel _timer;
        private IGameDataService _dataService;
        private float _nextBonusTime;
        private bool _bonusActive;

        private void Awake()
        {
            _timer = new TimerModel();

            _commonBonusDuration = bonusSettings.commonBonusDuration;
            _rareBonusDuration = bonusSettings.rareBonusDuration;
            _veryRareBonusDuration = bonusSettings.veryRareBonusDuration;

            _commonBonusChance = bonusSettings.commonBonusChance;
            _rareBonusChance = bonusSettings.rareBonusChance;
            _veryRareBonusChance = bonusSettings.veryRareBonusChance;

            _bonusCooldown = bonusSettings.bonusCooldown;

            #if UNITY_EDITOR
            Assert.IsNotNull(commonBonusUI, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(rareBonusUI, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(veryRareBonusUI, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(bonusSettings, $"{nameof(BonusSettings)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            _dataService = ServiceLocator.GameDataService;

            EventBus.Subscribe<GameStartEvent>(StartTimer);
            EventBus.Subscribe<GamePauseEvent>(PauseTimer);
            EventBus.Subscribe<GameResumeEvent>(ResumeTimer);
            EventBus.Subscribe<GameEndEvent>(StopTimer);

            TurnOffAllUI();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartEvent>(StartTimer);
            EventBus.Unsubscribe<GamePauseEvent>(PauseTimer);
            EventBus.Unsubscribe<GameResumeEvent>(ResumeTimer);
            EventBus.Unsubscribe<GameEndEvent>(StopTimer);
        }

        private void StartTimer()
        {
            float duration = _dataService?.GameTimerDuration ?? 0f;
            _timer.Initialize(duration);
            _timer.Start();
            _nextBonusTime = Time.time + _bonusCooldown;
        }

        private void PauseTimer()
        {
            _timer.Pause();
        }

        private void ResumeTimer()
        {
            _timer.Resume();
        }

        private void StopTimer()
        {
            _timer.Stop();
            CancelInvoke(nameof(EndBonus));
            _bonusActive = false;
            TurnOffAllUI();
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);

            if (_timer.IsRunning && !_bonusActive && Time.time >= _nextBonusTime)
            {
                TurnOffAllUI();
                TrySpawnBonus();
                _nextBonusTime = Time.time + _bonusCooldown;
            }
        }

        private void TrySpawnBonus()
        {
            float roll = Random.value;
            if (roll <= _veryRareBonusChance)
                ActivateBonus(BonusRarity.VeryRare, _veryRareBonusDuration, veryRareBonusUI);
            else if (roll <= _veryRareBonusChance + _rareBonusChance)
                ActivateBonus(BonusRarity.Rare, _rareBonusDuration, rareBonusUI);
            else if (roll <= _veryRareBonusChance + _rareBonusChance + _commonBonusChance)
                ActivateBonus(BonusRarity.Common, _commonBonusDuration, commonBonusUI);
        }

        private void ActivateBonus(BonusRarity rarity, float duration, GameObject ui)
        {
            _bonusActive = true;
            ui.SetActive(true);
            EventBus.Raise(new BonusStartEvent(rarity));
            Invoke(nameof(EndBonus), duration);
        }

        private void EndBonus()
        {
            _bonusActive = false;
            TurnOffAllUI();
            EventBus.Raise(new BonusEndEvent());
        }

        private void TurnOffAllUI()
        {
            commonBonusUI.SetActive(false);
            rareBonusUI.SetActive(false);
            veryRareBonusUI.SetActive(false);
        }
    }
}