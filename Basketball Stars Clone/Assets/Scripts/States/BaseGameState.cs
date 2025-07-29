using System;

namespace States
{
    public abstract class BaseGameState : IState
    {
        public event Action OnStateEntered;
        public event Action OnStateUpdated;
        public event Action OnStateExited;
        
        public virtual void Enter() => OnStateEntered?.Invoke();
        public virtual void Update() => OnStateUpdated?.Invoke();
        public virtual void Exit() => OnStateExited?.Invoke();
    }
}