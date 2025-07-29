using Enums;
using Events;
using Gameplay.Basketball;
using Gameplay.Throw;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gameplay.Player.Human
{
    /// <summary>
    /// Represents a player.
    /// Manages throw spots, ball throwing logic, animations and visual effects.
    /// Listens to game and throw-related events to coordinate player actions and state.
    /// </summary>
    public class Player : MonoBehaviour
    {
        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        private static readonly int ThrowTrigger = Animator.StringToHash("ThrowTrigger");
        
        [Header("Player")]
        [SerializeField] private PlayerNumber playerNumber = PlayerNumber.Player1;
        
        [Header("Camera parameters")]
        [SerializeField] private ThrowCamera playerCamera;
        [SerializeField] private Transform lookAtTarget;
        [SerializeField] private float offsetFromBall = 0.1f;
        
        [Header("Throw spots")]
        [SerializeField] private ThrowSpotHandler spotHandler;
        
        [Header("Ball")]
        [SerializeField] private BasketBall basketBall;
        [SerializeField] private BallHitDetector ballHitDetector;
        [SerializeField] private GameObject fireballVFX;
        
        protected IPlayerDataService PlayerDataService;
        protected IScoreDataService ScoreDataService;
        
        private ThrowSpot _nextSpot;
        private TargetType _lastTargetType;
        private Animator _animator;

        protected virtual void Start()
        {
            #if UNITY_EDITOR
            if (playerNumber == PlayerNumber.Player1)
            {
                Assert.IsNotNull(playerCamera, $"{nameof(ThrowCamera)} reference is missing on '{gameObject.name}'");
            }
            else
            {
                Assert.IsNull(playerCamera, $"{nameof(ThrowCamera)} must be null on '{gameObject.name}'");
            }
            Assert.IsNotNull(lookAtTarget, $"{nameof(Transform)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(spotHandler, $"{nameof(ThrowSpotHandler)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(basketBall, $"{nameof(BasketBall)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(ballHitDetector, $"{nameof(BallHitDetector)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(fireballVFX, $"{nameof(GameObject)} reference is missing on '{gameObject.name}'");
            #endif
            
            PlayerDataService = ServiceLocator.PlayerDataService;
            ScoreDataService = ServiceLocator.ScoreDataService;
            
            //_nextSpot = spotHandler.GetNextThrowSpot(playerNumber);
            //LoadSpot();
            
            Material material = PlayerDataService.GetBallMaterial(playerNumber);
            basketBall.SetMeshMaterial(material);
            
            GameObject characterPrefab = PlayerDataService.GetCharacterPrefab(playerNumber);
            var character = Instantiate(characterPrefab, transform);
            
            character.transform.localScale = new Vector3(0.63f, 0.63f, 0.63f);
            character.transform.localPosition = new Vector3(0, 2, 0);
            
            _animator = character.GetComponent<Animator>();
            _animator.SetBool(IsIdle, false);
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<FireballStartEvent>(TurnOnFireballVFX);
            EventBus.Subscribe<FireballExpiredEvent>(TurnOffFireballVFX);
            EventBus.Subscribe<MissEvent>(TurnOffFireballVFX);
            EventBus.Subscribe<GameStartEvent>(OnGameStart);

            ballHitDetector.OnBallResult += HandleBallResult;
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<FireballStartEvent>(TurnOnFireballVFX);
            EventBus.Unsubscribe<FireballExpiredEvent>(TurnOffFireballVFX);
            EventBus.Unsubscribe<MissEvent>(TurnOffFireballVFX);
            EventBus.Unsubscribe<GameStartEvent>(OnGameStart);
            
            ballHitDetector.OnBallResult -= HandleBallResult;
        }
        
        public virtual void SwipeRequest(TargetType targetType)
        {
            var velocity = BallTrajectory.ComputeVelocity(targetType, _nextSpot);
            basketBall.ThrowBall(velocity);
            
            _lastTargetType = targetType;

            if (playerCamera)
            {
                playerCamera.ApplyMovement();
            }
            
            _animator.SetTrigger(ThrowTrigger);
            EventBus.Raise(new BallThrowStartedEvent(playerNumber));
            ServiceLocator.SoundService.PlayRandomSound(SoundType.SFX_THROWBALL);
        }

        protected virtual void HandleBallResult(bool success)
        {
            ScoreDataService.ThrowResult(playerNumber, _lastTargetType, success);
            EventBus.Raise(new BallThrowEndedEvent(playerNumber));
            
            if (success)
            {
                _nextSpot = spotHandler.GetNextThrowSpot(playerNumber);
            }
            
            LoadSpot();
        }
        
        protected virtual void LoadSpot()
        {
            Vector3 posAdj = _nextSpot.StartPos;
            posAdj.y = 0;
            transform.position = posAdj;
            
            var targetAdj = lookAtTarget.position;
            targetAdj.y = transform.position.y;
            
            transform.LookAt(-targetAdj);
            transform.position -= transform.forward * offsetFromBall;
            
            basketBall.ResetBall(_nextSpot.StartPos);
            
            if (playerCamera)
            {
                playerCamera.ResetCameraTransform(_nextSpot.StartPos);
            }
            
            EventBus.Raise(new ThrowSpotLoadedEvent(playerNumber, _nextSpot.ThrowSpotData, _nextSpot.StartPos));
        }
        
        private void OnGameStart()
        {
            _nextSpot = spotHandler.GetNextThrowSpot(playerNumber);
            LoadSpot();
            TurnOffFireballVFX(new FireballExpiredEvent(playerNumber));
        }

        protected virtual void TurnOnFireballVFX(FireballStartEvent args)
        {
            if (args.Player != playerNumber) return;

            fireballVFX.SetActive(true);
            ServiceLocator.SoundService.PlaySound(SoundType.SFX_FIREBALL);
        }

        protected virtual void TurnOffFireballVFX(FireballExpiredEvent args)
        {
            if (args.Player != playerNumber) return;
            
            fireballVFX.SetActive(false);
        }

        protected virtual void TurnOffFireballVFX(MissEvent args)
        {
            if (args.Player != playerNumber) return;
            
            fireballVFX.SetActive(false);
        }
    }
}