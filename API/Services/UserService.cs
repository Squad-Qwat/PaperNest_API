using API.Models;
using API.Repositories;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace API.Services
{
    public class UserService
    {
        public void Create(User newUser)
        {
            Debug.Assert(newUser != null, "New User can not null here");

            int initialLength = UserRepository.userRepository.Count;

            UserRepository.userRepository.Add(newUser);

            Debug.Assert(UserRepository.userRepository.Count == initialLength + 1, "User was not added correctly");
        }

        public IEnumerable<User> GetAll()
        {
            var users = UserRepository.userRepository;

            Debug.Assert(users != null, "User list should not be null");

            return users;
        }

        public User? GetById(Guid userId)
        {
            Debug.Assert(userId != Guid.Empty, "User Id can not be empty");

            return UserRepository.userRepository.FirstOrDefault(u => u.Id == userId);

        }

        public User? Register(string? userEmail, string? userPassword, string? userName, string? userUsername, string? userRole = "Mahasiswa")
        {
            Debug.Assert(userEmail != string.Empty, "User email can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");
            Debug.Assert(userName != string.Empty, "User name can not be empty");
            Debug.Assert(userUsername != string.Empty, "User username can not be empty");
            Debug.Assert(userRole != string.Empty, "User role can not be empty");

            // Separate null checks for each field to provide specific error messages
            if (userEmail == null)
            {
               Debug.WriteLine("User registration failed: 'userEmail' fields are null.");
               return null;
            }
            
            if (userPassword == null)
            {
                Debug.WriteLine("User registration failed: 'userPassword' fields are null.");
                return null;
            }
            
            if (userName == null)
            {
                Debug.WriteLine("User registration failed: 'userName' fields are null.");
                return null;
            }
            
            if (userUsername == null)
            {
                Debug.WriteLine("User registration failed: 'userUsername' fields are null.");
                return null;
            }
            
            if (userRole == null)
            {
                Debug.WriteLine("User registration failed: 'userRole' fields are null.");
                return null;
            }

            var checkEmail = GetByEmail(userEmail);
            var checkUsername = GetByUsername(userUsername);

            Debug.Assert(checkEmail == null, "User email already exists");
            Debug.Assert(checkUsername == null, "User username already exists");

            var newUser = new User
            {
                Email = userEmail,
                Password = userPassword,
                Name = userName,
                Username = userUsername,
                Role = userRole,
                UpdateAt = DateTime.Now
            };
            Create(newUser);
            return newUser;
        }

        public User? GetByEmail(string userEmail)
        {
            Debug.Assert(userEmail != string.Empty, "User email can not be empty");
            return UserRepository.userRepository.FirstOrDefault(u => u.Email == userEmail);
        }

        public User? GetByUsername(string userUsername)
        {
            Debug.Assert(userUsername != string.Empty, "User username can not be empty");
            return UserRepository.userRepository.FirstOrDefault(u => u.Username == userUsername);
        }

        public User? GetByPassword(string userPassword)
        {
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");
            return UserRepository.userRepository.FirstOrDefault(u => u.Password == userPassword);
        }

        /*
         * Unused, due to the fact that we are just using Username instead of Email
         * public User? GetByIdOrEmailOrUsername(Guid userId, string userEmailOrUsername)
         * {
         *      Debug.Assert(userId != Guid.Empty, "User Id can not be empty");
         *      Debug.Assert(userEmailOrUsername != string.Empty, "User email or username can not be empty");
         *      return UserRepository.userRepository.FirstOrDefault(u => u.Id == userId || u.Email == userEmailOrUsername || u.Username == userEmailOrUsername);
         * }
         * 
         * public User? GetByEmailOrUsername(string userEmailOrUsername)
         * {
         *      Debug.Assert(userEmailOrUsername != string.Empty, "User email or username can not be empty");
         *      return UserRepository.userRepository.FirstOrDefault(u => u.Email == userEmailOrUsername || u.Username == userEmailOrUsername);
         * }
        */

        public User? GetByUsernameAndPassword(string userUsername, string userPassword)
        {
            Debug.Assert(userUsername != string.Empty, "User username can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");
            return UserRepository.userRepository.FirstOrDefault(u => u.Username == userUsername && u.Password == userPassword);
        }

        public User? GetByEmailAndPassword(string userEmail, string userPassword)
        {
            Debug.Assert(userEmail != string.Empty, "User username can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");
            return UserRepository.userRepository.FirstOrDefault(u => u.Username == userEmail && u.Password == userPassword);
        }

        /* 
         * Unused, due to the fact that we are just using Username instead of Email
         * public User? GetByEmailOrUsernameAndPassword(string userEmailOrUsername, string userPassword)
         * {
         *      Debug.Assert(userEmailOrUsername != string.Empty, "User email or username can not be empty");
         *      Debug.Assert(userPassword != string.Empty, "User password can not be empty");
         *      return UserRepository.userRepository.FirstOrDefault(u => (u.Email == userEmailOrUsername || u.Username == userEmailOrUsername) && u.Password == userPassword);
         * }
        */

        public User? Login(string? userUsername, string? userPassword)
        {
            Debug.Assert(userUsername != string.Empty, "Username can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");

            // Separate null checks for each field to provide specific error messages
            if (userUsername == null)
            {
                Debug.WriteLine("Login failed: Username field is null.");
                return null;
            }
            if (userPassword == null)
            {
                Debug.WriteLine("Login failed: user password field is null.");
                return null;
            }

            return GetByUsernameAndPassword(userUsername, userPassword);
        }

        // Only for AuthController.cs
        public User? LoginAuth(string? userEmail, string? userPassword)
        {
            Debug.Assert(userEmail != string.Empty, "Username can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");

            // Separate null checks for each field to provide specific error messages
            if (userEmail == null)
            {
                Debug.WriteLine("Login failed: Username field is null.");
                return null;
            }
            if (userPassword == null)
            {
                Debug.WriteLine("Login failed: user password field is null.");
                return null;
            }

            return GetByEmailAndPassword(userEmail, userPassword);
        }

        public void Update(Guid userId, User newUser)
        {
            Debug.Assert(userId != Guid.Empty, "User Id can not be empty");

            var existingUser = GetById(userId);

            Debug.Assert(existingUser != null, "Filtered User should be not empty");

            if (existingUser != null)
            {
                existingUser.Name = newUser.Name;
                existingUser.Username = newUser.Username;

                existingUser.UpdateAt = DateTime.Now;
            }
        }

        public bool ResetPassword(string userEmail, string newUserPassword)
        {
            try
            {
                var existingUser = UserRepository.userRepository.FirstOrDefault(u => u.Email == userEmail) ??
                throw new InvalidOperationException("User with this email does not exist");
                /*
                 * Setara dengan:
                 * if (existingUser == null)
                 * {
                 *    throw new InvalidOperationException("User with this email does not exist");
                 * }
                 */
                existingUser.Password = newUserPassword;

                existingUser.UpdateAt = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error resetting password: {ex.Message}");
                return false;
            }
        }

        public void ChangeEmail(string oldUserEmail, string newUserEmail)
        {
            try
            {
                Debug.Assert(newUserEmail != string.Empty, "User email can not be empty");

                var existingUser = UserRepository.userRepository.FirstOrDefault(u => u.Email == oldUserEmail) ?? 
                    throw new InvalidOperationException("Your email is empty");
                /*
                 * Setara dengan:
                    if (existingUser == null)
                    {
                        throw new InvalidOperationException("Your email is empty");
                    }
                */

                Debug.Assert(existingUser != null, "Filtered User should be not empty");

                var emailExists = UserRepository.userRepository.Any(u => u.Email == newUserEmail);

                if (emailExists)
                {
                    throw new InvalidOperationException("New email has been used another user");
                }
                existingUser.Email = newUserEmail;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error changing email: {ex.Message}");
            }
        }

        public void Delete(Guid deletedUserId)
        {
            try
            {
                Debug.Assert(deletedUserId != Guid.Empty, "User Id can not be empty");

                var existingUser = GetById(deletedUserId);

                Debug.Assert(existingUser != null, "Filtered User should be not empty");

                if (existingUser == null)
                {
                    throw new InvalidOperationException("User not found");
                }
                UserRepository.userRepository.Remove(existingUser);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
            }
        }
    }
}