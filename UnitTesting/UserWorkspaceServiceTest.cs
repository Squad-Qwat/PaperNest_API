using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using API.Services;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class UserWorkspaceServiceTest
    {
        private UserWorkspaceService _userWorkspaceService;

        [TestInitialize]
        public void Setup()
        {
            _userWorkspaceService = new UserWorkspaceService();
            UserWorkspaceRepository.UserWorkspace.Clear();
        }

        #region GetAllUserWorkspaces
        [TestMethod]
        public void GetAllUserWorkspaces_ReturnsAllUserWorkspaces()
        {
            // Arrange
            var userWorkspace1 = new UserWorkspace 
            { 
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };
            var userWorkspace2 = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Owner
            };
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace1);
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace2);

            // Act
            var result = _userWorkspaceService.GetAllUserWorkspaces();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, userWorkspace1);
            CollectionAssert.Contains(result, userWorkspace2);
        }

        [TestMethod]
        public void GetAllUserWorkspaces_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _userWorkspaceService.GetAllUserWorkspaces();

            // Assert
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region GetUserWorkspaceById
        [TestMethod]
        public void GetUserWorkspaceById_WhenExists_ReturnsUserWorkspace()
        {
            // Arrange
            var userWorkspace = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace);

            // Act
            var result = _userWorkspaceService.GetUserWorkspaceById(userWorkspace.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userWorkspace.Id, result.Id);
            Assert.AreEqual(userWorkspace.WorkspaceRole, result.WorkspaceRole);
        }

        [TestMethod]
        public void GetUserWorkspaceById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = _userWorkspaceService.GetUserWorkspaceById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserWorkspaceById_WithEmptyGuid_ThrowsArgumentException()
        {
            _userWorkspaceService.GetUserWorkspaceById(Guid.Empty);
        }
        #endregion

        #region GetUserWorkspacesByUserId
        [TestMethod]
        public void GetUserWorkspacesByUserId_WhenExists_ReturnsUserWorkspaces()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userWorkspace1 = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };
            var userWorkspace2 = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Owner
            };
            var otherUserWorkspace = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };

            UserWorkspaceRepository.AddUserWorkspace(userWorkspace1);
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace2);
            UserWorkspaceRepository.AddUserWorkspace(otherUserWorkspace);

            // Act
            var result = _userWorkspaceService.GetUserWorkspacesByUserId(userId).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, userWorkspace1);
            CollectionAssert.Contains(result, userWorkspace2);
            CollectionAssert.DoesNotContain(result, otherUserWorkspace);
        }

        [TestMethod]
        public void GetUserWorkspacesByUserId_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = _userWorkspaceService.GetUserWorkspacesByUserId(Guid.NewGuid()).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserWorkspacesByUserId_WithEmptyGuid_ThrowsArgumentException()
        {
            _userWorkspaceService.GetUserWorkspacesByUserId(Guid.Empty);
        }
        #endregion

        #region AddUserWorkspaceAsOwner
        [TestMethod]
        public void AddUserWorkspaceAsOwner_CreatesNewUserWorkspace()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            // Act
            _userWorkspaceService.AddUserWorkspaceAsOwner(userId, workspaceId);

            // Assert
            var userWorkspace = UserWorkspaceRepository.UserWorkspace.First();
            Assert.AreEqual(userId, userWorkspace.FK_UserId);
            Assert.AreEqual(workspaceId, userWorkspace.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Owner, userWorkspace.WorkspaceRole);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUserWorkspaceAsOwner_WithEmptyUserId_ThrowsArgumentException()
        {
            _userWorkspaceService.AddUserWorkspaceAsOwner(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUserWorkspaceAsOwner_WithEmptyWorkspaceId_ThrowsArgumentException()
        {
            _userWorkspaceService.AddUserWorkspaceAsOwner(Guid.NewGuid(), Guid.Empty);
        }
        #endregion

        #region AddUserWorkspaceAsMember
        [TestMethod]
        public void AddUserWorkspaceAsMember_CreatesNewUserWorkspace()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            // Act
            _userWorkspaceService.AddUserWorkspaceAsMember(userId, workspaceId);

            // Assert
            var userWorkspace = UserWorkspaceRepository.UserWorkspace.First();
            Assert.AreEqual(userId, userWorkspace.FK_UserId);
            Assert.AreEqual(workspaceId, userWorkspace.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Member, userWorkspace.WorkspaceRole);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUserWorkspaceAsMember_WithEmptyUserId_ThrowsArgumentException()
        {
            _userWorkspaceService.AddUserWorkspaceAsMember(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUserWorkspaceAsMember_WithEmptyWorkspaceId_ThrowsArgumentException()
        {
            _userWorkspaceService.AddUserWorkspaceAsMember(Guid.NewGuid(), Guid.Empty);
        }
        #endregion

        #region AddUserWorkspaceAsLecturer
        [TestMethod]
        public void AddUserWorkspaceAsLecturer_CreatesNewUserWorkspace()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            // Act
            _userWorkspaceService.AddUserWorkspaceAsLecturer(userId, workspaceId);

            // Assert
            var userWorkspace = UserWorkspaceRepository.UserWorkspace.First();
            Assert.AreEqual(userId, userWorkspace.FK_UserId);
            Assert.AreEqual(workspaceId, userWorkspace.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Lecturer, userWorkspace.WorkspaceRole);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUserWorkspaceAsLecturer_WithEmptyUserId_ThrowsArgumentException()
        {
            _userWorkspaceService.AddUserWorkspaceAsLecturer(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUserWorkspaceAsLecturer_WithEmptyWorkspaceId_ThrowsArgumentException()
        {
            _userWorkspaceService.AddUserWorkspaceAsLecturer(Guid.NewGuid(), Guid.Empty);
        }

        [TestMethod]
        public void RemoveUserWorkspace_ValidIds_ReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();
            var userWorkspace = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspaceId,
                WorkspaceRole = WorkspaceRole.Member
            };
            UserWorkspaceRepository.AddUserWorkspace(userWorkspace);
            
            var result = _userWorkspaceService.RemoveUserWorkspace(userId, workspaceId);
         
            Assert.IsTrue(result);
            Assert.IsFalse(UserWorkspaceRepository.UserWorkspace.Contains(userWorkspace));
        }

        [TestMethod]
        public void RemoveUserWorkspace_InvalidIds_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();
            var result = _userWorkspaceService.RemoveUserWorkspace(userId, workspaceId);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveUserWorkspace_WithEmptyUserId_ThrowsArgumentException()
        {
            _userWorkspaceService.RemoveUserWorkspace(Guid.Empty, Guid.NewGuid());
        }

        #endregion
    }
}