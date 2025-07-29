namespace States
{
    public class BaseStateMachine
    {
        public BaseGameState CurrentState { get; private set; }
        
        public void ChangeState(BaseGameState newState)
        {
            if (newState == null) return;
        
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void ResetState()
        {
            CurrentState = null;
        }

        public void UpdateState() => CurrentState?.Update();
    }
}