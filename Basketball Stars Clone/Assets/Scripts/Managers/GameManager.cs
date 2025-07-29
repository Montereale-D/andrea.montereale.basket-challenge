using Enums;
using Events;
using Interfaces;
using States;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Central controller for managing game flow and state transitions.
    /// Initializes and handles the game state machine, reacts to key gameplay events,
    /// and coordinates transitions between phases.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private BaseStateMachine _stateMachine;
        private LoadingState _loadingState;
        private PlayState _playState;
        private PauseState _pauseState;
        private WaitEndingState _waitEndingState;
        private RecapState _recapState;

        private bool _isBallLocked1 = true;
        private bool _isBallLocked2 = true;
        
        private static GameManager Instance { get; set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;

            _stateMachine = new BaseStateMachine();
            
            _loadingState = new LoadingState();
            _playState = new PlayState();
            _pauseState = new PauseState();
            _waitEndingState = new WaitEndingState();
            _recapState = new RecapState();

            _loadingState.OnStateEntered += OnLoadingStart;
            _playState.OnStateEntered += OnPlayStart;
            _pauseState.OnStateEntered += OnPauseStart;
            _pauseState.OnStateExited += OnPauseEnd;
            _recapState.OnStateEntered += OnRecapStart;
        }
        
        private void OnEnable()
        {
            EventBus.Subscribe<TimeEndEvent>(OnTimeEnd);
            EventBus.Subscribe<BallThrowStartedEvent>(OnBallThrowStart);
            EventBus.Subscribe<BallThrowEndedEvent>(OnBallThrowEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<TimeEndEvent>(OnTimeEnd);
            EventBus.Unsubscribe<BallThrowStartedEvent>(OnBallThrowStart);
            EventBus.Unsubscribe<BallThrowEndedEvent>(OnBallThrowEnd);
        }
        
        private void Start()
        {
            _stateMachine.ChangeState(_loadingState);
        }
        
        private void OnLoadingStart()
        {
            EventBus.Raise(new GameStartEvent());
            _stateMachine.ChangeState(_playState);
        }
        
        private void OnPlayStart()
        {
            Time.timeScale = 1;
        }
        
        private void OnPauseStart()
        {
            Time.timeScale = 0;
            EventBus.Raise(new GamePauseEvent());
        }
        
        private void OnPauseEnd()
        {
            EventBus.Raise(new GameResumeEvent());
        }

        private void OnRecapStart()
        {
            Time.timeScale = 0;
            IScoreDataService scoreService = ServiceLocator.ScoreDataService;
            var score1 = scoreService.GetPlayerScore(PlayerNumber.Player1);
            var score2 = scoreService.GetPlayerScore(PlayerNumber.Player2);
            EventBus.Raise(new GameEndEvent(score1, score2));
        }
        
        public void Restart()
        {
            _stateMachine.ChangeState(_loadingState);
        }

        public void Pause()
        {
            _stateMachine.ChangeState(_pauseState);
        }

        public void Resume()
        {
            _stateMachine.ChangeState(_playState);
        }
        
        private void OnBallThrowStart(BallThrowStartedEvent args)
        {
            if (args.PlayerNumber == PlayerNumber.Player1)
            {
                _isBallLocked1 = false;
            }
            else
            {
                _isBallLocked2 = false;
            }
        }

        private void OnBallThrowEnd(BallThrowEndedEvent args)
        {
            if (args.PlayerNumber == PlayerNumber.Player1)
            {
                _isBallLocked1 = true;
            }
            else
            {
                _isBallLocked2 = true;
            }

            if (IsInWaitEndingState() && _isBallLocked1 && _isBallLocked2)
            {
                _stateMachine.ChangeState(_recapState);
            }
        }

        private void OnTimeEnd()
        {
            if (!IsInPlayState()) return;
            
            if (_isBallLocked1 && _isBallLocked2)
            {
                _stateMachine.ChangeState(_recapState);
            }
            else
            {
                _stateMachine.ChangeState(_waitEndingState);
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            
            _loadingState.OnStateEntered -= OnLoadingStart;
            _playState.OnStateEntered -= OnPlayStart;
            _pauseState.OnStateEntered -= OnPauseStart;
            _pauseState.OnStateExited -= OnPauseEnd;
            _recapState.OnStateEntered -= OnRecapStart;
            
            _stateMachine.ResetState();
            EventBus.Clear();
        }
        
        public bool IsInPlayState() => _stateMachine.CurrentState is PlayState;
        public bool IsInPauseState() => _stateMachine.CurrentState is PauseState;
        public bool IsInWaitEndingState() => _stateMachine.CurrentState is WaitEndingState;
        public bool IsInRecapState() => _stateMachine.CurrentState is RecapState;
    }
}
