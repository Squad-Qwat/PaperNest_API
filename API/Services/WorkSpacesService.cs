using API.Helpers.Enums;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class WorkspaceService
    {
        public void Create(Workspace newWorkspace)
        {
            try
            {
                if (newWorkspace == null)
                {
                    throw new ArgumentNullException(nameof(newWorkspace), "Workspace cannot be null.");
                }

                WorkspaceRepository.workspaceRepository.Add(newWorkspace);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error creating workspace: {ex.Message}");
            }
        }

        public IEnumerable<Workspace> GetAll()
        {
            if (WorkspaceRepository.workspaceRepository == null || !WorkspaceRepository.workspaceRepository.Any())
            {
                Console.WriteLine("No workspaces found.");
                return []; // Setara dengan 'Enumerable.Empty<Workspace>()'
            }

            return WorkspaceRepository.workspaceRepository;
        }

        public Workspace? GetById(Guid workspaceId)
        {
            if (workspaceId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve workspace with empty ID.");
                return null;
            }

            return WorkspaceRepository.workspaceRepository.FirstOrDefault(w => w.Id == workspaceId);
        }



        public IEnumerable<Workspace> GetByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve workspaces for empty user ID.");
                return []; // Setara dengan 'Enumerable.Empty<Workspace>()'
            }

            var userWorkspace = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            var workspaceIds = userWorkspace.Select(uw => uw.FK_WorkspaceId).ToList();
            var workspaces = WorkspaceRepository.workspaceRepository
                .Where(w => workspaceIds.Contains(w.Id))
                .ToList();
            return workspaces;
        }

        public UserWorkspace? JoinWorkspace(Guid workspaceId, Guid userId, WorkspaceRole role = WorkspaceRole.Lecturer)
        {
            if (workspaceId == Guid.Empty || userId == Guid.Empty)
            {
                Console.WriteLine("Cannot join workspace with empty ID.");
                return null;
            }

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
            if (workspaceId == Guid.Empty || newWorkspace == null)
            {
                Console.WriteLine("Cannot update workspace with empty ID or null workspace.");
                return;
            }

            var existingWorkspace = GetById(workspaceId);

            if (existingWorkspace == null)
            {
                Console.WriteLine("Workspace not found.");
                return;
            }

            existingWorkspace.Title = newWorkspace.Title;
            existingWorkspace.Description = newWorkspace.Description;

            existingWorkspace.UpdateAt = DateTime.Now;
        }

        public void Delete(Guid deletedWorkspaceId)
        {
            if (deletedWorkspaceId == Guid.Empty)
            {
                Console.WriteLine("Cannot delete workspace with empty ID.");
                return;
            }

            var existingWorkspace = GetById(deletedWorkspaceId);

            if (existingWorkspace == null)
            {
                Console.WriteLine("Workspace not found.");
                return;
            }

            WorkspaceRepository.workspaceRepository.Remove(existingWorkspace);
        }

        public IEnumerable<Workspace> GetJoinedWorkspaces(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve joined workspaces for empty user ID.");
                return []; // Setara dengan 'Enumerable.Empty<Workspace>()'
            }

            // Gunakan UserWorkspaceRepository untuk mendapatkan workspace yang diikuti oleh user
            var userWorkspaces = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId)
                .Select(uw => uw.FK_WorkspaceId);

            return WorkspaceRepository.workspaceRepository.Where(w => userWorkspaces.Contains(w.Id));
        }

        public WorkspaceRole GetUserRoleInWorkspace(Guid userId, Guid workspaceId)
        {
            if (userId == Guid.Empty || workspaceId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve user role with empty user ID or workspace ID.");
                return WorkspaceRole.Member; // Default role if parameters are invalid
            }

            var userWorkspaces = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            var userWorkspace = userWorkspaces.FirstOrDefault(uw => uw.FK_WorkspaceId == workspaceId);

            if (userWorkspace == null)
            {
                return WorkspaceRole.Member; // Default role if user is not part of the workspace
            }
            
            return userWorkspace.WorkspaceRole;
        }
    }
}