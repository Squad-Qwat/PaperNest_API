using static API.StateMachineAndUtils.AuthStateMachine;

namespace API.StateMachineAndUtils
{
    public class Transition(AuthStateMachine.AuthState prevState, AuthStateMachine.AuthState nextState, AuthStateMachine.Trigger trigger)
    {
        public AuthState PrevState { get; } = prevState;
        public AuthState NextState { get; } = nextState;
        public Trigger Trigger { get; } = trigger;
        
        /*
         * Setara dengan:
         * public AuthState PrevState { get; }
         * public AuthState NextState { get; }
         * public Trigger Trigger { get; }
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