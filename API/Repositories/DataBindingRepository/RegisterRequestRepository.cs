using API.Models.DataBinding;
using System.Net.NetworkInformation;

namespace API.Repositories.DataBindingRepository
{
    // Only used in Global View, no need for service class
    public class RegisterRequestRepository
    {
        private static readonly List<RegisterRequest> _registerRequests = [];
        private static RegisterRequest? registerRequest;

        public RegisterRequestRepository()
        {
            registerRequest = new RegisterRequest();
        }

        public static List<RegisterRequest> RegisterRequest
        {
            get => _registerRequests;
            set
            {
                if (value != null)
                {
                    _registerRequests.Clear();
                    _registerRequests.AddRange(value);
                }
            }
        }

        public RegisterRequest? GetRegisterRequest()
        {
            return registerRequest;
        }

        // Get methods for RegisterRequest based on different properties, returning the attribute instead of the whole class
        public static Guid GetIdRegisterRequest(Guid id)
        {
            // return _registerRequests.FirstOrDefault(x => x.Id == id) ?? new RegisterRequest();
            return _registerRequests.FirstOrDefault(x => x.Id == id)?.Id ?? Guid.Empty;
        }

        public static string? GetNameRegisterRequest(string? name)
        {
            // return _registerRequests.FirstOrDefault(x => x.Name == name) ?? new RegisterRequest();
            return _registerRequests.FirstOrDefault(x => x.Name == name)?.Name ?? null;
        }

        public static string? GetEmailRegisterRequest(string? email)
        {
            // return _registerRequests.FirstOrDefault(x => x.Email == email) ?? new RegisterRequest();
            return _registerRequests.FirstOrDefault(x => x.Email == email)?.Email ?? null;
        }
        public static string? GetUsernameRegisterRequest(string? username)
        {
            // return _registerRequests.FirstOrDefault(x => x.Username == username) ?? new RegisterRequest();
            return _registerRequests.FirstOrDefault(x => x.Username == username)?.Username ?? null;
        }

        public static string? GetPasswordRegisterRequest(string? password)
        {
            // return _registerRequests.FirstOrDefault(x => x.Password == password) ?? new RegisterRequest();
            return _registerRequests.FirstOrDefault(x => x.Password == password)?.Password ?? null;
        }

        public static string? GetRoleRegisterRequest(string? role)
        {
            // return _registerRequests.FirstOrDefault(x => x.Role == role) ?? new RegisterRequest();
            return _registerRequests.FirstOrDefault(x => x.Role == role)?.Role ?? null;
        }
    }
}