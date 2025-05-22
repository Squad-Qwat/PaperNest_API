using API.Models;
using API.Helpers.Enums;
// using System.Diagnostics;

namespace API.Repositories
{
    public class UserWorkspaceRepository
    {
        public static List<UserWorkspace> _userWorkspace= []; // Setara dengan 'new List<UserWorkspace>()'

        public static List<UserWorkspace> UserWorkspace
        {
            get => _userWorkspace;
            set
            {
                try
                {
                    if (value == null)
                    {
                        Console.WriteLine("Setting new user workspace list.");
                        return;
                    }

                    if (value.Count == 0)
                    {
                        Console.WriteLine("Setting empty user workspace list.");
                    }
                    else
                    {
                        Console.WriteLine($"Setting user workspace list with {value.Count} items.");
                    }

                    if (value.Any(c => c == null))
                    {
                        throw new ArgumentException("User workspace list cannot contain null items.");
                    }

                    if (value.Any(c => c.Id == Guid.Empty))
                    {
                        throw new ArgumentException("User workspace list cannot contain items with empty IDs.");
                    }
                    _userWorkspace.Clear(); // Clear existing data
                    _userWorkspace.AddRange(value); // Add new data
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error setting user workspace: {ex.Message}");
                }
            }
        }

        public static UserWorkspace? GetUserWorkspaceById(Guid id)
        {
            if (id == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve user workspace with empty ID.");
                return null;
            }

            return _userWorkspace.FirstOrDefault(uw => uw.Id == id);
        }

        public static void AddUserWorkspace(UserWorkspace userWorkspace)
        {
            try
            {
                if (userWorkspace == null)
                {
                    throw new ArgumentNullException(nameof(userWorkspace), "User workspace cannot be null.");
                }
                _userWorkspace.Add(userWorkspace);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error adding user workspace: {ex.Message}");
            }
        }

        public static IEnumerable<UserWorkspace> GetUserWorkspacesByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve user workspaces with empty user ID.");
                return []; // Setara dengan 'Enumerable.Empty<UserWorkspace>()'
            }
            return _userWorkspace.Where(uw => uw.FK_UserId == userId);
        }

        /*
        public static UserWorkspace? UpdateUserWorkspace(Guid userId, Guid workspaceId)
        {
            try
            {
                if (userId == Guid.Empty || workspaceId == Guid.Empty)
                {
                    throw new ArgumentException("User ID and workspace ID cannot be empty.");
                }
                var userWorkspace = _userWorkspace.FirstOrDefault(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId);
                if (userWorkspace == null)
                {
                    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                }
                // userWorkspace.WorkspaceRole = newRole;
                userWorkspace.UpdateAt = DateTime.Now; // Update timestamp
                return userWorkspace;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating user workspace: {ex.Message}");
                return null;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating user workspace: {ex.Message}");
                return null;
            }
        }
        */

        public static bool UpdateUserWorkspace(Guid userId, Guid workspaceId)
        {
            try
            {
                if (userId == Guid.Empty || workspaceId == Guid.Empty)
                {
                    throw new ArgumentException("User ID and workspace ID cannot be empty.");
                }

                var userWorkspace = _userWorkspace.FirstOrDefault(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId) ??
                    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                /*
                 * Setara dengan 
                 * if (userWorkspace == null)
                 * {
                 *    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                 * }
                 */
                userWorkspace.UpdateAt = DateTime.Now; // Update timestamp
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating user workspace: {ex.Message}");
                return false;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating user workspace: {ex.Message}");
                return false;
            }
        }

        /*
        public static UserWorkspace? UpdateUserWorkspaceRole(Guid userId, Guid workspaceId, WorkspaceRole role) 
        {
            try
            {
                if (userId == Guid.Empty || workspaceId == Guid.Empty)
                {
                    throw new ArgumentException("User ID and workspace ID cannot be empty.");
                }
                var userWorkspace = _userWorkspace.FirstOrDefault(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId) ?? 
                    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                {
                 * Setara dengan:
                 * if (userWorkspace == null)
                 * {
                 *    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                 * }
                }
                userWorkspace.WorkspaceRole = role;
                userWorkspace.UpdateAt = DateTime.Now; // Update timestamp
                return userWorkspace;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating user workspace: {ex.Message}");
                return null;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating user workspace: {ex.Message}");
                return null;
            }
        }
        */

        public static bool UpdateUserWorkspaceRole(Guid userId, Guid workspaceId, WorkspaceRole role)
        {
            try
            {
                if (userId == Guid.Empty || workspaceId == Guid.Empty)
                {
                    throw new ArgumentException("User ID and workspace ID cannot be empty.");
                }
                var userWorkspace = _userWorkspace.FirstOrDefault(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId) ??
                    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                /*
                 * Setara dengan 
                 * if (userWorkspace == null)
                 * {
                 *    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                 * }
                 */
                userWorkspace.WorkspaceRole = role;
                userWorkspace.UpdateAt = DateTime.Now; // Update timestamp
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating user workspace role: {ex.Message}");
                return false;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error updating user workspace role: {ex.Message}");
                return false;
            }
        }

        public static bool RemoveUserWorkspace(Guid userId, Guid workspaceId)
        {
            try
            {
                if (userId == Guid.Empty || workspaceId == Guid.Empty)
                {
                    throw new ArgumentException("Cannot remove user workspace with empty user ID or workspace ID.");
                }
                if (!_userWorkspace.Any(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId))
                {
                    throw new KeyNotFoundException("User workspace not found for the given user ID and workspace ID.");
                }

                var userWorkspace = _userWorkspace.FirstOrDefault(uw => uw.FK_UserId == userId && uw.FK_WorkspaceId == workspaceId) ??
                    throw new KeyNotFoundException("User workspace not found.");
                /*
                 * Setara dengan 
                 * if (userWorkspace == null)
                 * {
                 *    throw new KeyNotFoundException("User workspace not found.");
                 * }
                 */
                _userWorkspace.Remove(userWorkspace);
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error removing user workspace: {ex.Message}");
                return false;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error removing user workspace: {ex.Message}");
                return false;
            }
        }
    }
}