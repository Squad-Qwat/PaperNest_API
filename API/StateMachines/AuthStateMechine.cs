namespace API.StateMachines
{
    public class AuthStateMachine
    {
        public enum AuthState { BELUM_LOGIN, SUDAH_LOGIN };
        public enum Trigger { LOGIN, LOGOUT };

        // where the transition used to be

        private AuthState currentState;
        private readonly List<Transition> transitions;

        public AuthStateMachine()
        {
            currentState = AuthState.BELUM_LOGIN;
            transitions =
            // Equivalent to 'new List<Transition>(){}'
            [
                new(AuthState.BELUM_LOGIN, AuthState.SUDAH_LOGIN, Trigger.LOGIN), // Equivalent to 'new Transition(AuthState.BELUM_LOGIN, AuthState.SUDAH_LOGIN, Trigger.LOGIN)'
                new(AuthState.SUDAH_LOGIN, AuthState.BELUM_LOGIN, Trigger.LOGOUT), // Equivalent to 'new Transition(AuthState.SUDAH_LOGIN, AuthState.BELUM_LOGIN, Trigger.LOGOUT)'
                new(AuthState.BELUM_LOGIN, AuthState.BELUM_LOGIN, Trigger.LOGOUT), // Equivalent to 'new Transition(AuthState.BELUM_LOGIN, AuthState.BELUM_LOGIN, Trigger.LOGOUT)'
                new(AuthState.SUDAH_LOGIN, AuthState.SUDAH_LOGIN, Trigger.LOGIN) // Equivalent to 'new Transition(AuthState.SUDAH_LOGIN, AuthState.SUDAH_LOGIN, Trigger.LOGIN)'
            ];
        }

        private AuthState GetNextState(AuthState prevState, Trigger trigger)
        {
            foreach (var transition in transitions)
            {
                if (transition.PrevState == prevState && transition.Trigger == trigger)
                {
                    return transition.NextState;
                }
            }
            return prevState;
        }

        public void ActivateTrigger(Trigger trigger)
        {
            AuthState nextState = GetNextState(currentState, trigger);
            currentState = nextState;
            Console.WriteLine($"Status sekarang: {currentState}");
        }

        public AuthState GetCurrentState()
        {
            return currentState;
        }
    }
}