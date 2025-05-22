using API.Helpers.Enums;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class UserWorkspaceService
    {
        public List<UserWorkspace> GetAllUserWorkspaces()
        {

            return UserWorkspaceRepository.UserWorkspace;
        }

        public UserWorkspace? GetUserWorkspaceById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id tidak boleh kosong", nameof(id));
            }
            return UserWorkspaceRepository.GetUserWorkspaceById(id);
        }

        public IEnumerable<UserWorkspace> GetUserWorkspacesByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
            }
            return UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
        }

        public void AddUserWorkspaceAsOwner(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
            }
            if (workspaceId == Guid.Empty)
            {
                throw new ArgumentException("WorkspaceId tidak boleh kosong", nameof(workspaceId));
            }
            var userWorkspace = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspaceId,
                WorkspaceRole = WorkspaceRole.Owner,
            };
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace);
        }

        public void AddUserWorkspaceAsMember(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
            }
            if (workspaceId == Guid.Empty)
            {
                throw new ArgumentException("WorkspaceId tidak boleh kosong", nameof(workspaceId));
            }
            var userWorkspace = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspaceId,
                WorkspaceRole = WorkspaceRole.Member,
            };
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace);
        }

        public void AddUserWorkspaceAsLecturer(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
            }
            if (workspaceId == Guid.Empty)
            {
                throw new ArgumentException("WorkspaceId tidak boleh kosong", nameof(workspaceId));
            }
            var userWorkspace = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspaceId,
                WorkspaceRole = WorkspaceRole.Lecturer,
            };
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace);
        }

        public bool RemoveUserWorkspace(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
            }
            if (workspaceId == Guid.Empty)
            {
                throw new ArgumentException("WorkspaceId tidak boleh kosong", nameof(workspaceId));
            }
            return UserWorkspaceRepository.RemoveUserWorkspace(userId, workspaceId);
        }
    }
}
