using API.Models;
using API.Repositories;
using API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class UserServiceTests
    {
        private UserService? _userService;

        [TestInitialize]
        public void Setup()
        {
            _userService = new UserService();
            UserRepository.userRepository.Clear();
        }

        #region Create
        [TestMethod]
        public void Create_AddsNewUser_Successfully()
        {
            // Arrange
            var newUser = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                Username = "testuser",
                Password = "password123"
            };

            // Act
            _userService.Create(newUser);

            // Assert
            Assert.AreEqual(1, UserRepository.userRepository.Count);
            Assert.IsTrue(UserRepository.userRepository.Contains(newUser));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_WithNullUser_ThrowsArgumentNullException()
        {
            _userService.Create(null);
        }
        #endregion

        #region GetAll
        [TestMethod]
        public void GetAll_ReturnsAllUsers()
        {
            // Arrange
            var user1 = new User { Name = "User1", Email = "user1@example.com" };
            var user2 = new User { Name = "User2", Email = "user2@example.com" };
            UserRepository.userRepository.Add(user1);
            UserRepository.userRepository.Add(user2);

            // Act
            var result = _userService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            CollectionAssert.Contains(result.ToList(), user1);
            CollectionAssert.Contains(result.ToList(), user2);
        }

        [TestMethod]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _userService.GetAll();

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion

        #region GetById
        [TestMethod]
        public void GetById_WhenExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Name = "Test User", Email = "test@example.com" };
            UserRepository.userRepository.Add(user);

            // Act
            var result = _userService.GetById(user.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.Name, result.Name);
        }

        [TestMethod]
        public void GetById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = _userService.GetById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetById_WithEmptyGuid_ThrowsArgumentException()
        {
            _userService.GetById(Guid.Empty);
        }
        #endregion

        #region Update
        [TestMethod]
        public void Update_WhenUserExists_UpdatesUser()
        {
            // Arrange
            var existingUser = new User
            {
                Name = "Old Name",
                Username = "oldusername"
            };
            UserRepository.userRepository.Add(existingUser);

            var updatedUser = new User
            {
                Name = "New Name",
                Username = "newusername"
            };

            // Act
            _userService.Update(existingUser.Id, updatedUser);

            // Assert
            Assert.AreEqual("New Name", existingUser.Name);
            Assert.AreEqual("newusername", existingUser.Username);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Update_WithEmptyGuid_ThrowsArgumentException()
        {
            _userService.Update(Guid.Empty, new User());
        }
        #endregion

        #region ResetPassword
        [TestMethod]
        public void ResetPassword_WhenUserExists_ReturnsTrue()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Password = "oldpassword"
            };
            UserRepository.userRepository.Add(user);

            // Act
            var result = _userService.ResetPassword("test@example.com", "newpassword");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("newpassword", user.Password);
        }

        [TestMethod]
        public void ResetPassword_WhenUserNotExists_ReturnsFalse()
        {
            // Act
            var result = _userService.ResetPassword("nonexistent@example.com", "newpassword");

            // Assert
            Assert.IsFalse(result);
        }
        #endregion

        #region ChangeEmail
        [TestMethod]
        public void ChangeEmail_WhenValidNewEmail_UpdatesEmail()
        {
            // Arrange
            var user = new User { Email = "old@example.com" };
            UserRepository.userRepository.Add(user);

            // Act
            _userService.ChangeEmail("old@example.com", "new@example.com");

            // Assert
            Assert.AreEqual("new@example.com", user.Email);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangeEmail_WhenNewEmailExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var user1 = new User { Email = "user1@example.com" };
            var user2 = new User { Email = "user2@example.com" };
            UserRepository.userRepository.Add(user1);
            UserRepository.userRepository.Add(user2);

            // Act
            _userService.ChangeEmail("user1@example.com", "user2@example.com");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangeEmail_WhenOldEmailNotFound_ThrowsInvalidOperationException()
        {
            _userService.ChangeEmail("nonexistent@example.com", "new@example.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeEmail_WithEmptyNewEmail_ThrowsArgumentException()
        {
            _userService.ChangeEmail("old@example.com", string.Empty);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void Delete_WhenUserExists_RemovesUser()
        {
            // Arrange
            var user = new User { Name = "Test User" };
            UserRepository.userRepository.Add(user);
            var userId = user.Id;

            // Act
            _userService.Delete(userId);

            // Assert
            Assert.AreEqual(0, UserRepository.userRepository.Count);
            Assert.IsNull(_userService.GetById(userId));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Delete_WithEmptyGuid_ThrowsArgumentException()
        {
            _userService.Delete(Guid.Empty);
        }
        #endregion
    }
}
