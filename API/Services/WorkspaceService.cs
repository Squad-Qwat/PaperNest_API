using API.Models;
using API.Repositories;

namespace PaperNest_API.Services
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

        public static Workspace? GetById(Guid id)
        {
            return WorkspaceRepository.workspaceRepository.FirstOrDefault(w => w.Id == id);
        }

        public static IEnumerable<Workspace> GetByUserId(Guid userId)
        {
            return WorkspaceRepository.workspaceRepository.Where(uw => uw.UserWorkspaces.Any(w => w.FK_UserId == userId));
        }
    }
}