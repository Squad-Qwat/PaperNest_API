using API.Models;

namespace API.Repositories
{
    public class UserWorkspaceRepository
    {
        public static List<UserWorkspace> _userWorkspace= new List<UserWorkspace>();

        public static List<UserWorkspace> UserWorkspace
        {
            get => _userWorkspace;
            set
            {
                if (value != null)
                {
                    _userWorkspace.Clear();
                    _userWorkspace.AddRange(value);
                }
            }
        }

        public static UserWorkspace? GetUserWorkspaceById(Guid id)
        {
            return _userWorkspace.FirstOrDefault(uw => uw.Id == id);
        }

        public static void AddUserWorkspace(UserWorkspace userWorkspace)
        {
            if (userWorkspace != null)
            {
                _userWorkspace.Add(userWorkspace);
            }
        }

        public static IEnumerable<UserWorkspace> GetUserWorkspacesByUserId(Guid userId)
        {
            return _userWorkspace.Where(uw => uw.FK_UserId == userId);
        }

        public static bool RemoveUserWorkspace(Guid userId, Guid workspaceId)
        {
            var userWorkspace = _userWorkspace.FirstOrDefault(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId);
            if (userWorkspace != null)
            {
                _userWorkspace.Remove(userWorkspace);
                return true;
            }
            return false;
        }
    }
}
