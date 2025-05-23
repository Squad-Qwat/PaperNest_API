namespace API.StateMachines
{
    public class AuthStateMachine
    {
        public enum AuthState { BELUM_LOGIN, SUDAH_LOGIN };
        public enum Trigger { LOGIN, LOGOUT };

        public class Transition
        {
            public AuthState PrevState { get; }
            public AuthState NextState { get; }
            public Trigger Trigger { get; }

            public Transition(AuthState prevState, AuthState nextState, Trigger trigger)
            {
                PrevState = prevState;
                NextState = nextState;
                Trigger = trigger;
            }
        }

        private AuthState currentState;
        private List<Transition> transitions;

        public AuthStateMachine()
        {
            currentState = AuthState.BELUM_LOGIN;
            transitions = new List<Transition>
            {
                new Transition(AuthState.BELUM_LOGIN, AuthState.SUDAH_LOGIN, Trigger.LOGIN),
                new Transition(AuthState.SUDAH_LOGIN, AuthState.BELUM_LOGIN, Trigger.LOGOUT),
                new Transition(AuthState.BELUM_LOGIN, AuthState.BELUM_LOGIN, Trigger.LOGOUT),
                new Transition(AuthState.SUDAH_LOGIN, AuthState.SUDAH_LOGIN, Trigger.LOGIN)
            };
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

