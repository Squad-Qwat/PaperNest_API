using API.Models;

namespace API.Repositories
{
    public class UserRepository
    {
        public static List<User> userRepository = []; // Setara dengan 'new List<User>()'

        public static List<UserWorkspace> userWorkspaceRepository = []; // Setara dengan 'new List<UserWorkspace>()'
    }
}
