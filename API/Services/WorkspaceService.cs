using API.Helpers.Enums;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class WorkspaceService
    {
        public static void Create(Workspace newWorkspace)
        {
            WorkspaceRepository.workspaceRepository.Add(newWorkspace);
        }

        public static IEnumerable<Workspace> GetAll()
        {
            return WorkspaceRepository.workspaceRepository;
        }

        public static Workspace? GetById(Guid workspaceId)
        {
            return WorkspaceRepository.workspaceRepository.FirstOrDefault(w => w.Id == workspaceId);
        }

        public static IEnumerable<Workspace> GetByUserId(Guid userId)
        {
            return WorkspaceRepository.workspaceRepository.Where(uw => uw.UserWorkspaces.Any(w => w.FK_UserId == userId));
        }

        public static UserWorkspace? JoinWorkspace(Guid workspaceId, Guid userId, WorkspaceRole role = WorkspaceRole.Lecturer)
        {
            var workspace = GetById(workspaceId);

            if (workspace == null)
            {
                return null;
            }
            
            var existingUserWorkspace = UserRepository.userWorkspaceRepository
                .FirstOrDefault(uw => uw.Id == workspaceId && uw.FK_UserId == userId);
                
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
            
            UserRepository.userWorkspaceRepository.Add(userWorkspace);
            return userWorkspace;
        }

        public static void Update(Guid workspaceId, Workspace newWorkspace)
        {
            var existingWorkspace = GetById(workspaceId);

            if (existingWorkspace != null)
            {
                existingWorkspace.Title = newWorkspace.Title;
                existingWorkspace.Description = newWorkspace.Description;

                existingWorkspace.UpdateAt = DateTime.Now;
            }
        }

        public static void Delete(Guid deletedWorkspaceId)
        {
            var existingWorkspace = GetById(deletedWorkspaceId);

            if (existingWorkspace != null)
            {
                WorkspaceRepository.workspaceRepository.Remove(existingWorkspace);
            }
        }
        
        public static IEnumerable<Workspace> GetJoinedWorkspaces(Guid userId)
        {
            var userWorkspaces = UserRepository.userWorkspaceRepository
                .Where(uw => uw.FK_UserId == userId)
                .Select(uw => uw.FK_WorkspaceId);
                
            return WorkspaceRepository.workspaceRepository.Where(w => userWorkspaces.Contains(w.Id));
        }
    }
}