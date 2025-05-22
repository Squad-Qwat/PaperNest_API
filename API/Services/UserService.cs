using API.Models;
using API.Repositories;
using System.Diagnostics;

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

        public User Register(string userEmail, string userPassword, string userName, string userUsername, string userRole = "Mahasiswa")
        {
            Debug.Assert(userEmail != string.Empty, "User email can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");
            Debug.Assert(userName != string.Empty, "User name can not be empty");
            Debug.Assert(userUsername != string.Empty, "User username can not be empty");
            Debug.Assert(userRole != string.Empty, "User role can not be empty");

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

        public User? GetByEmailOrUsernameAndPassword(string userEmailOrUsername, string userPassword)
        {
            Debug.Assert(userEmailOrUsername != string.Empty, "User email or username can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");
            return UserRepository.userRepository.FirstOrDefault(u => (u.Email == userEmailOrUsername || u.Username == userEmailOrUsername) && u.Password == userPassword);
        }

        public User Login(string userEmailOrUsername, string userPassword)
        {
            Debug.Assert(userEmailOrUsername != string.Empty, "User email or username can not be empty");
            Debug.Assert(userPassword != string.Empty, "User password can not be empty");

            return GetByEmailOrUsernameAndPassword(userEmailOrUsername, userPassword);
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
            var existingUser = UserRepository.userRepository.FirstOrDefault(u => u.Email == userEmail);

            if (existingUser != null)
            {
                existingUser.Password = newUserPassword;

                existingUser.UpdateAt = DateTime.Now;
                return true;
            }

            return false;
        }

        public void ChangeEmail(string oldUserEmail, string newUserEmail)
        {
            Debug.Assert(newUserEmail != string.Empty, "User email can not be empty");

            var existingUser = UserRepository.userRepository.FirstOrDefault(u => u.Email == oldUserEmail);

            Debug.Assert(existingUser != null, "Filtered User should be not empty");

            if (existingUser != null)
            {
                var emailExists = UserRepository.userRepository.Any(u => u.Email == newUserEmail);

                if (!emailExists)
                {
                    existingUser.Email = newUserEmail;
                }
                else
                {
                    throw new InvalidOperationException("New email has been used another user");
                }
            }
            else
            {
                throw new InvalidOperationException("Your email is empty");
            }
        }

        public void Delete(Guid deletedUserId)
        {
            Debug.Assert(deletedUserId != Guid.Empty, "User Id can not be empty");

            var existingUser = GetById(deletedUserId);

            Debug.Assert(existingUser != null, "Filtered User should be not empty");

            if (existingUser != null)
            {
                UserRepository.userRepository.Remove(existingUser);
            }
        }
    }
}
