using static API.StateMachines.AuthStateMachine;

namespace API.StateMachines
{
    public class Transition(AuthStateMachine.AuthState prevState, AuthStateMachine.AuthState nextState, AuthStateMachine.Trigger trigger)
    {
        public AuthState PrevState { get; } = prevState;
        public AuthState NextState { get; } = nextState;
        public Trigger Trigger { get; } = trigger;

        /*
         * Equivalent to:
         *  public AuthState PrevState { get; }
         *  public AuthState NextState { get; }
         *  public Trigger Trigger { get; }
         *  
         * public Transition(AuthState prevState, AuthState nextState, Trigger trigger)
         * {
         *    PrevState = prevState;
         *    NextState = nextState;
         *    Trigger = trigger;
         * }
         */
    }
}