using API.Helpers.Enums;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class UserWorkspaceService
    {
        public List<UserWorkspace> GetAllUserWorkspaces()
        {
            try
            {
                if (UserWorkspaceRepository.UserWorkspace == null)
                {
                    throw new InvalidOperationException("User workspace repository is not initialized.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Count == 0)
                {
                    throw new InvalidOperationException("Tidak ada data user workspace yang tersedia.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c == null))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang kosong.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.Id == Guid.Empty))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang ID-nya kosong.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.FK_UserId == Guid.Empty || c.FK_WorkspaceId == Guid.Empty))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang FK_UserId atau FK_WorkspaceId-nya kosong.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.CreatedAt == default || c.UpdateAt == default))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang CreatedAt atau UpdateAt-nya kosong.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.UpdateAt < c.CreatedAt))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang UpdateAt lebih kecil dari CreatedAt.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.UpdateAt > DateTime.Now))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang UpdateAt lebih besar dari waktu sekarang.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.CreatedAt > DateTime.Now))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang CreatedAt lebih besar dari waktu sekarang.");
                }
                if (UserWorkspaceRepository.UserWorkspace.Any(c => c.Workspace == null || c.User == null))
                {
                    throw new InvalidOperationException("Data user workspace tidak boleh ada yang Workspace atau User-nya kosong.");
                }
                return UserWorkspaceRepository.UserWorkspace;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error retrieving user workspaces: {ex.Message}");
                return []; // Setara dengan 'new List<UserWorkspace>()'
            }
        }

        public UserWorkspace? GetUserWorkspaceById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Id tidak boleh kosong", nameof(id));
                }
                return UserWorkspaceRepository.GetUserWorkspaceById(id);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving user workspace by ID: {ex.Message}");
                return null;
            }
        }

        public IEnumerable<UserWorkspace> GetUserWorkspacesByUserId(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
                }
                return UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving user workspaces by UserId: {ex.Message}");
                return []; // Setara dengan 'Enumerable.Empty<UserWorkspace>()'
            }
        }

        public void AddUserWorkspaceAsOwner(Guid userId, Guid workspaceId)
        {
            try
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
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error adding user workspace as owner: {ex.Message}");
            }
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
            try
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
            catch (ArgumentException ex) 
            {
                Console.WriteLine($"Error adding user workspace as lecturer: {ex.Message}");
            }
        }
        public bool RemoveUserWorkspace(Guid userId, Guid workspaceId)
        {
            try
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
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error removing user workspace: {ex.Message}");
                return false;
            }
        }
    }
}