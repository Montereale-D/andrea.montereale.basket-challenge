using Data;
using Enums;
using Events;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Gameplay.Swipe
{
    /// <summary>
    /// Handles swipe accuracy visualization and evaluation.
    /// Updates UI slider with swipe progress and shows accuracy target zones (Perfect and Backboard).
    /// Determines swipe accuracy category based on swipe value and predefined target ranges,
    /// and requests corresponding swipe action on the player.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class SwipeAccuracyHandler : MonoBehaviour
    {
        [SerializeField] private Player.Human.Player player;
        [SerializeField] private RectTransform perfectArea;
        [SerializeField] private RectTransform backboardArea;
    
        private Slider _slider;
    
        private float _minPerfectRange;
        private float _maxPerfectRange;
        private float _minBackboardRange;
        private float _maxBackboardRange;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            
            #if UNITY_EDITOR
            Assert.IsNotNull(perfectArea, $"{nameof(RectTransform)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(backboardArea, $"{nameof(RectTransform)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SwipeUpdateEvent>(OnSwipeUpdate);
            EventBus.Subscribe<SwipeOccuredEvent>(CheckSwipeAccuracy);
            EventBus.Subscribe<ThrowSpotLoadedEvent>(LoadThrowSpot);
        }
        private void OnDisable()
        {
            EventBus.Unsubscribe<SwipeUpdateEvent>(OnSwipeUpdate);
            EventBus.Unsubscribe<SwipeOccuredEvent>(CheckSwipeAccuracy);
            EventBus.Unsubscribe<ThrowSpotLoadedEvent>(LoadThrowSpot);
        }

        private void LoadThrowSpot(ThrowSpotLoadedEvent args)
        {
            if (args.Player != PlayerNumber.Player1) return;
        
            _slider.value = 0;
            UpdateAccuracyAreaUI(args.ThrowSpotData.perfectAccuracyData, perfectArea, TargetType.Perfect);
            UpdateAccuracyAreaUI(args.ThrowSpotData.backboardAccuracyData, backboardArea, TargetType.Backboard);
        }


        private void UpdateAccuracyAreaUI(SwipeAccuracyData swipeAccuracyData, RectTransform accuracyRect, TargetType targetType)
        {
            RectTransform sliderRect = _slider.GetComponent<RectTransform>();
        
            float sliderWidth = sliderRect.rect.width;
        
            accuracyRect.sizeDelta = new Vector2(swipeAccuracyData.areaWidth, accuracyRect.sizeDelta.y);
        
            float newPosX = Mathf.Lerp(0, sliderWidth, swipeAccuracyData.verticalPosition);
        
            float startPercent = swipeAccuracyData.verticalPosition - (swipeAccuracyData.areaWidth / 2 / sliderWidth);
            float endPercent = swipeAccuracyData.verticalPosition + (swipeAccuracyData.areaWidth / 2 / sliderWidth);

            if (targetType == TargetType.Perfect)
            {
                _minPerfectRange = startPercent;
                _maxPerfectRange = endPercent;
            }
            else if (targetType == TargetType.Backboard)
            {
                _minBackboardRange = startPercent;
                _maxBackboardRange = endPercent;
            }
            else
            {
                Debug.LogError("Wrong enum value");
            }
        
            #if UNITY_EDITOR
            float maxPerfectPosition = sliderWidth - (swipeAccuracyData.areaWidth / 2);
            float minPerfectPosition = (swipeAccuracyData.areaWidth / 2);
            Assert.IsTrue(newPosX >= minPerfectPosition && newPosX <= maxPerfectPosition);
            #endif
        
            accuracyRect.anchoredPosition = new Vector2(newPosX, 0);
        }

        private void CheckSwipeAccuracy(SwipeOccuredEvent args)
        {
            float swipe = args.Value;
        
            if (swipe >= _minPerfectRange && swipe <= _maxPerfectRange)
            {
                Debug.Log("CheckSwipeAccuracy "  + TargetType.Perfect);
                player.SwipeRequest(TargetType.Perfect);
            }
            else if (swipe >= _minBackboardRange && swipe <= _maxBackboardRange)
            {
                Debug.Log("CheckSwipeAccuracy "  + TargetType.Backboard);
                player.SwipeRequest(TargetType.Backboard);
            }
            else
            {
                float perfectCenter = (_minPerfectRange + _maxPerfectRange) / 2f;
                float backboardCenter = (_minBackboardRange + _maxBackboardRange) / 2f;
                
                float distanceToPerfect = Mathf.Abs(swipe - perfectCenter);
                float distanceToBackboard = Mathf.Abs(swipe - backboardCenter);
                
                if (swipe < _minPerfectRange)
                {
                    player.SwipeRequest(TargetType.UnderPerfect);
                }
                else if (swipe > _maxBackboardRange)
                {
                    player.SwipeRequest(TargetType.OverBackboard);
                }
                else
                {
                    player.SwipeRequest(TargetType.UnderBackboard);
                }
            }
        }
    
        private void OnSwipeUpdate(SwipeUpdateEvent arg)
        {
            _slider.value = arg.Value;
        }
    }
}