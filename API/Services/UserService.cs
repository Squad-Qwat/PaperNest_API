using API.Models;
using API.Repositories;
using System.Diagnostics;

namespace API.Services
{
    public class UserService
    {
        public static void Create(User newUser)
        {
            Debug.Assert(newUser != null, "New User can not null here");

            int initialLength = UserRepository.userRepository.Count;

            UserRepository.userRepository.Add(newUser);

            Debug.Assert(UserRepository.userRepository.Count == initialLength + 1, "User was not added correctly");
        }

        public static IEnumerable<User> GetAll()
        {
            var users = UserRepository.userRepository;

            Debug.Assert(users != null, "User list should not be null");

            return users;
        }

        public static User? GetById(Guid userId)
        {
            Debug.Assert(userId != Guid.Empty, "User Id can not be empty");

            return UserRepository.userRepository.FirstOrDefault(u => u.Id == userId);

        }

        public static void Update(Guid userId, User newUser)
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

        public static void ResetPassword(string userEmail, string newUserPassword)
        {
            Debug.Assert(userEmail != string.Empty, "User email can not be empty");

            Debug.Assert(newUserPassword != string.Empty, "New password can not be empty");

            var existingUser = UserRepository.userRepository.FirstOrDefault(u => u.Email == userEmail);

            Debug.Assert(existingUser != null, "Filtered User should be not empty");

            if (existingUser != null)
            {
                existingUser.Password = newUserPassword;

                existingUser.UpdateAt = DateTime.Now;
            }
        }

        public static void ChangeEmail(String newUserEmail)
        {
            Debug.Assert(newUserEmail != string.Empty, "User email can not be empty");

            var existingUser = UserRepository.userRepository.FirstOrDefault(u => u.Email == newUserEmail);

            Debug.Assert(existingUser != null, "Filtered User should be not empty");

            if (existingUser != null)
            {
                existingUser.Email = newUserEmail;
            }
        }

        public static void Delete(Guid deletedUserId)
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
