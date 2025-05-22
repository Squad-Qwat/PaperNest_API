using API.Helpers.Enums;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class WorkspaceService
    {
        public void Create(Workspace newWorkspace)
        {
            WorkspaceRepository.workspaceRepository.Add(newWorkspace);
        }

        public IEnumerable<Workspace> GetAll()
        {
            return WorkspaceRepository.workspaceRepository;
        }

        public Workspace? GetById(Guid workspaceId)
        {
            return WorkspaceRepository.workspaceRepository.FirstOrDefault(w => w.Id == workspaceId);
        }



        public IEnumerable<Workspace> GetByUserId(Guid userId)
        {
            var userWorkspace = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            var workspaceIds = userWorkspace.Select(uw => uw.FK_WorkspaceId).ToList();
            var workspaces = WorkspaceRepository.workspaceRepository
                .Where(w => workspaceIds.Contains(w.Id))
                .ToList();
            return workspaces;
        }

        public UserWorkspace? JoinWorkspace(Guid workspaceId, Guid userId, WorkspaceRole role = WorkspaceRole.Lecturer)
        {
            var workspace = GetById(workspaceId);

            if (workspace == null)
            {
                return null;
            }

            // Gunakan UserWorkspaceRepository daripada akses langsung ke UserRepository
            var existingUserWorkspace = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId)
                .FirstOrDefault(uw => uw.FK_WorkspaceId == workspaceId);

            if (existingUserWorkspace != null)
            {
                return existingUserWorkspace;
            }

            var userWorkspace = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspaceId,
                WorkspaceRole = role,
                UpdateAt = DateTime.Now
            };

            // Gunakan UserWorkspaceRepository daripada akses langsung ke UserRepository
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace);
            return userWorkspace;
        }

        public void Update(Guid workspaceId, Workspace newWorkspace)
        {
            var existingWorkspace = GetById(workspaceId);

            if (existingWorkspace != null)
            {
                existingWorkspace.Title = newWorkspace.Title;
                existingWorkspace.Description = newWorkspace.Description;

                existingWorkspace.UpdateAt = DateTime.Now;
            }
        }

        public void Delete(Guid deletedWorkspaceId)
        {
            var existingWorkspace = GetById(deletedWorkspaceId);

            if (existingWorkspace != null)
            {
                WorkspaceRepository.workspaceRepository.Remove(existingWorkspace);
            }
        }

        public IEnumerable<Workspace> GetJoinedWorkspaces(Guid userId)
        {
            // Gunakan UserWorkspaceRepository untuk mendapatkan workspace yang diikuti oleh user
            var userWorkspaces = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId)
                .Select(uw => uw.FK_WorkspaceId);

            return WorkspaceRepository.workspaceRepository.Where(w => userWorkspaces.Contains(w.Id));
        }

        public WorkspaceRole GetUserRoleInWorkspace(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty || workspaceId == Guid.Empty)
            {
                return WorkspaceRole.Member;
            }

            var userWorkspaces = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            var userWorkspace = userWorkspaces.FirstOrDefault(uw => uw.FK_WorkspaceId == workspaceId);

            if (userWorkspace != null)
            {
                return userWorkspace.WorkspaceRole;
            }

            return WorkspaceRole.Member;
        }
    }
}
