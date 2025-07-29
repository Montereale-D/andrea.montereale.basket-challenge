using Enums;
using Events;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Gameplay.Swipe
{
    /// <summary>
    /// Manages swipe input detection and processing.
    /// Tracks swipe progress with thresholds and timing constraints,
    /// visualizes swipe trail, and raises corresponding swipe events.
    /// Supports pausing, resuming, and enabling/disabling detection via game events.
    /// </summary>
    public class SwipeController : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Camera swipeCamera;
        [SerializeField] private TrailRenderer trailRendererPrefab;
        
        [Header("Swipe Settings")]
        [SerializeField, Tooltip("Minimum swipe threshold as a fraction (0–1) of the screen height"), Range(0, 1)]
        private float screenSwipeThreshold = 0.1f;

        [SerializeField, Tooltip("Maximum normalized swipe distance"), Range(0, 1)]
        private float maxScreenSwipeDistance = 0.5f;

        [SerializeField, Tooltip("Maximum swipe time in seconds")]
        private float swipeTimeLimit = 1.5f;
        
        private const PlayerNumber Player = PlayerNumber.Player1;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _touchPositionAction;
        private InputAction _touchPressAction;
        private TrailRenderer _activeTrail;
        
        private Vector2 _startPosNormalized;
        
        private float _swipeStartTime;
        private float _swipePercentage;
        private bool _isSwiping;
        private bool _isDetectionEnable;
        private bool _swipeStarted;
        private bool _shouldActivate;
        
        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            
            #if UNITY_EDITOR
            Assert.IsNotNull(swipeCamera, $"Camera reference is missing in '{gameObject.name}'");
            Assert.IsNotNull(trailRendererPrefab, $"{nameof(GameObject)} reference is missing in '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            _touchPositionAction = _playerInputActions.Touch.TouchPosition;
            _touchPressAction = _playerInputActions.Touch.TouchPress;
            
            _touchPositionAction.Enable();
            _touchPressAction.Enable();

            _touchPressAction.performed += OnTouchPress;
            _touchPressAction.canceled += OnTouchRelease;
            
            EventBus.Subscribe<GameStartEvent>(TurnOn);
            EventBus.Subscribe<GamePauseEvent>(OnPause);
            EventBus.Subscribe<GameResumeEvent>(TurnOn);
            EventBus.Subscribe<TimeEndEvent>(OnEndTime);
            EventBus.Subscribe<ThrowSpotLoadedEvent>(OnSpotLoaded);
        }

        private void OnDisable()
        {
            _touchPressAction.performed -= OnTouchPress;
            _touchPressAction.canceled -= OnTouchRelease;
            
            _touchPositionAction.Disable();
            _touchPressAction.Disable();
            
            EventBus.Unsubscribe<GameStartEvent>(TurnOn);
            EventBus.Unsubscribe<GamePauseEvent>(OnPause);
            EventBus.Unsubscribe<GameResumeEvent>(TurnOn);
            EventBus.Unsubscribe<TimeEndEvent>(OnEndTime);
            EventBus.Unsubscribe<ThrowSpotLoadedEvent>(OnSpotLoaded);
        }
        
        private void Update()
        {
            if (!_isDetectionEnable) return;
            if (!_isSwiping) return;

            SwipeHandle();
        }
        
        private void SwipeHandle()
        {
            if (!_isDetectionEnable) return;
            
            Vector2 pos = _touchPositionAction.ReadValue<Vector2>();
            Vector2 posNormalized = swipeCamera.ScreenToViewportPoint(pos);
            
            if (_activeTrail)
            {
                Vector3 worldPos = swipeCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 1f));
                _activeTrail.transform.position = worldPos;
            }

            float swipeDelta = posNormalized.y - _startPosNormalized.y;
            float absDelta = Mathf.Abs(swipeDelta);

            // if swipeThreshold reached in this frame 
            if (!_swipeStarted && absDelta >= screenSwipeThreshold)
            {
                _swipeStarted = true;
                _swipeStartTime = Time.time;
            }

            float currentPercentage = Mathf.Clamp01(absDelta / maxScreenSwipeDistance);
            _swipePercentage = Mathf.Max(_swipePercentage, currentPercentage);
            EventBus.Raise(new SwipeUpdateEvent(_swipePercentage));
            
            if (!_swipeStarted) return;
            
            // backward swipe
            if (swipeDelta < 0f)
            {
                EndSwipe();
                return;
            }
            
            if (Time.time - _swipeStartTime > swipeTimeLimit)
            {
                EndSwipe();
            }
        }

        private void OnTouchPress(InputAction.CallbackContext ctx)
        {
            if (!_isDetectionEnable) return;
            
            Vector2 screenPos = _touchPositionAction.ReadValue<Vector2>();
            _startPosNormalized = swipeCamera.ScreenToViewportPoint(screenPos);

            _isSwiping = true;
            _swipeStarted = false;
            _swipePercentage = 0f;
            
            if (trailRendererPrefab)
            {
                if (_activeTrail) Destroy(_activeTrail.gameObject);
                Vector3 spawnPos = swipeCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 1f));
                _activeTrail = Instantiate(trailRendererPrefab, spawnPos, Quaternion.identity);
            }
        }
        
        private void OnTouchRelease(InputAction.CallbackContext ctx)
        {
            if (!_isDetectionEnable) return;
            
            if (_swipeStarted)
            {
                EndSwipe();
            }
            else
            {
                ResetSwipe();
            }
        }
        
        private void EndSwipe()
        {
            DisableDetection();
            EventBus.Raise(new SwipeOccuredEvent(Player, _swipePercentage));
            ResetSwipe();
        }

        private void ResetSwipe()
        {
            _isSwiping = false;
            _swipeStarted = false;
            _swipePercentage = 0f;
            
            if (_activeTrail)
            {
                Destroy(_activeTrail.gameObject);
                _activeTrail = null;
            }
        }
        
        private void OnPause()
        {
            _shouldActivate = false;
            DisableDetection();
        }

        private void OnEndTime()
        {
            DisableDetection();
            ResetSwipe();
        }
        
        private void OnSpotLoaded()
        {
            if (_shouldActivate)
            {
                TurnOn();
            }
        }

        private void TurnOn()
        {
            _isDetectionEnable = true;
            _shouldActivate = true;
        }

        private void DisableDetection()
        {
            _isDetectionEnable = false;
            _isSwiping = false;
            _swipeStarted = false;
            EventBus.Raise(new SwipeUpdateEvent(0));
        }
    }
}