using API.Models.DataBinding;

namespace API.Repositories.DataBindingRepository
{
    // Only used in Global View, no need for service class
    public class LoginRequestRepository
    {
        private static readonly List<LoginRequest> _loginRequests = [];
        private static LoginRequest? loginRequest;

        public LoginRequestRepository() 
        {
            loginRequest = new LoginRequest();
        }

        public static List<LoginRequest> LoginRequest 
        {
            get => _loginRequests;
            set 
            {
                if (value != null)
                {
                    _loginRequests.Clear();
                    _loginRequests.AddRange(value);
                }
            }
        }

        public LoginRequest? GetLoginRequest()
        {
            return loginRequest;
        }

        // Get methods for LoginRequest based on different properties, returning the attribute instead of the whole class
        public static string? GetEmailLoginRequest(string? email)
        {
            // return _loginRequests.FirstOrDefault(x => x.Email == email) ?? new LoginRequest();
            return _loginRequests.FirstOrDefault(x => x.Email == email)?.Email ?? null;
        }
        public static string? GetUsernameLoginRequest(string? username)
        {
            // return _loginRequests.FirstOrDefault(x => x.Username == username) ?? new LoginRequest();
            return _loginRequests.FirstOrDefault(x => x.Username == username)?.Username ?? null;
        }

        public static string? GetPasswordLoginRequest(string? password)
        {
            // return _loginRequests.FirstOrDefault(x => x.Password == password) ?? new LoginRequest();
            return _loginRequests.FirstOrDefault(x => x.Password == password)?.Password ?? null;
        }
    }
}