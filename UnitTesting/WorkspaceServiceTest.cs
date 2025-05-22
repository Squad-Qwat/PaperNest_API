using API.Helpers.Enums;
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
    public class WorkspaceServiceTests
    {
        private WorkspaceService? _workspaceService;

        [TestInitialize]
        public void Setup()
        {
            _workspaceService = new WorkspaceService();
            WorkspaceRepository.workspaceRepository.Clear();
            UserWorkspaceRepository._userWorkspace.Clear();
        }


        #region Create
        [TestMethod]
        public void Create_AddsNewWorkspace()
        {
            // Arrange
            var workspace = new Workspace
            {
                Title = "Test Workspace",
                Description = "Test Description"
            };

            // Act
            _workspaceService.Create(workspace);

            // Assert
            Assert.AreEqual(1, WorkspaceRepository.workspaceRepository.Count());
            Assert.IsTrue(WorkspaceRepository.workspaceRepository.Contains(workspace));
        }
        #endregion

        #region GetAll
        [TestMethod]
        public void GetAll_ReturnsAllWorkspaces()
        {
            // Arrange
            var workspace1 = new Workspace { Title = "Workspace 1" };
            var workspace2 = new Workspace { Title = "Workspace 2" };
            WorkspaceRepository.workspaceRepository.Add(workspace1);
            WorkspaceRepository.workspaceRepository.Add(workspace2);

            // Act
            var result = _workspaceService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            CollectionAssert.Contains(result.ToList(), workspace1);
            CollectionAssert.Contains(result.ToList(), workspace2);
        }

        [TestMethod]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _workspaceService.GetAll();

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion

        #region GetById
        [TestMethod]
        public void GetById_WhenExists_ReturnsWorkspace()
        {
            // Arrange
            var workspace = new Workspace
            {
                Title = "Test Workspace",
                Description = "Test Description"
            };
            WorkspaceRepository.workspaceRepository.Add(workspace);

            // Act
            var result = _workspaceService.GetById(workspace.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(workspace.Id, result.Id);
            Assert.AreEqual(workspace.Title, result.Title);
        }

        [TestMethod]
        public void GetById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = _workspaceService.GetById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }
        #endregion

        #region GetByUserId
        [TestMethod]
        public void GetByUserId_WhenExists_ReturnsUserWorkspaces()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workspace1 = new Workspace { Title = "Workspace 1" };
            var userWorkspace1 = new UserWorkspace { FK_UserId = userId, FK_WorkspaceId = workspace1.Id };
            workspace1.UserWorkspaces = new List<UserWorkspace> { userWorkspace1 };

            var workspace2 = new Workspace { Title = "Workspace 2" };
            var userWorkspace2 = new UserWorkspace { FK_UserId = userId, FK_WorkspaceId = workspace2.Id };
            workspace2.UserWorkspaces = new List<UserWorkspace> { userWorkspace2 };

            var otherWorkspace = new Workspace { Title = "Other Workspace" };

            WorkspaceRepository.workspaceRepository.Add(workspace1);
            WorkspaceRepository.workspaceRepository.Add(workspace2);
            WorkspaceRepository.workspaceRepository.Add(otherWorkspace);

            // Add the UserWorkspace objects to the UserWorkspaceRepository
            UserWorkspaceRepository._userWorkspace.Add(userWorkspace1);
            UserWorkspaceRepository._userWorkspace.Add(userWorkspace2);

            // Act
            var result = _workspaceService.GetByUserId(userId).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, workspace1);
            CollectionAssert.Contains(result, workspace2);
            CollectionAssert.DoesNotContain(result, otherWorkspace);
        }


        [TestMethod]
        public void GetByUserId_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = _workspaceService.GetByUserId(Guid.NewGuid());

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion

        #region JoinWorkspace
        [TestMethod]
        public void JoinWorkspace_WhenWorkspaceExists_CreatesUserWorkspace()
        {
            // Arrange
            var workspace = new Workspace { Title = "Test Workspace" };
            WorkspaceRepository.workspaceRepository.Add(workspace);
            var userId = Guid.NewGuid();

            // Act
            var result = _workspaceService.JoinWorkspace(workspace.Id, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.FK_UserId);
            Assert.AreEqual(workspace.Id, result.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Member, result.WorkspaceRole);
        }

        [TestMethod]
        public void JoinWorkspace_WhenUserAlreadyJoined_ReturnsExistingUserWorkspace()
        {
            // Arrange
            var workspace = new Workspace { Title = "Test Workspace" };
            WorkspaceRepository.workspaceRepository.Add(workspace);
            var userId = Guid.NewGuid();
            var existingUserWorkspace = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspace.Id,
                WorkspaceRole = WorkspaceRole.Member
            };
            UserRepository.userWorkspaceRepository.Add(existingUserWorkspace);

            // Act
            var result = _workspaceService.JoinWorkspace(workspace.Id, userId, existingUserWorkspace.WorkspaceRole);

            // Assert
            Assert.IsNotNull(result);
            // Compare properties instead of object references
            Assert.AreEqual(existingUserWorkspace.FK_UserId, result.FK_UserId);
            Assert.AreEqual(existingUserWorkspace.FK_WorkspaceId, result.FK_WorkspaceId);
            Assert.AreEqual(existingUserWorkspace.WorkspaceRole, result.WorkspaceRole);
        }

        [TestMethod]
        public void JoinWorkspace_WhenWorkspaceNotExists_ReturnsNull()
        {
            // Act
            var result = _workspaceService.JoinWorkspace(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }
        #endregion

        #region Update
        [TestMethod]
        public void Update_WhenWorkspaceExists_UpdatesWorkspace()
        {
            // Arrange
            var existingWorkspace = new Workspace
            {
                Title = "Old Title",
                Description = "Old Description"
            };
            WorkspaceRepository.workspaceRepository.Add(existingWorkspace);

            var updatedWorkspace = new Workspace
            {
                Title = "New Title",
                Description = "New Description"
            };

            // Act
            _workspaceService.Update(existingWorkspace.Id, updatedWorkspace);

            // Assert
            Assert.AreEqual("New Title", existingWorkspace.Title);
            Assert.AreEqual("New Description", existingWorkspace.Description);
            Assert.IsTrue(existingWorkspace.UpdateAt > DateTime.MinValue);
        }

        [TestMethod]
        public void Update_WhenWorkspaceNotExists_DoesNothing()
        {
            // Arrange
            var updatedWorkspace = new Workspace
            {
                Title = "New Title",
                Description = "New Description"
            };

            // Act
            _workspaceService.Update(Guid.NewGuid(), updatedWorkspace);

            // Assert
            Assert.AreEqual(0, WorkspaceRepository.workspaceRepository.Count());
        }
        #endregion

        #region Delete
        [TestMethod]
        public void Delete_WhenWorkspaceExists_RemovesWorkspace()
        {
            // Arrange
            var workspace = new Workspace { Title = "Test Workspace" };
            WorkspaceRepository.workspaceRepository.Add(workspace);

            // Act
            _workspaceService.Delete(workspace.Id);

            // Assert
            Assert.AreEqual(0, WorkspaceRepository.workspaceRepository.Count());
            Assert.IsNull(_workspaceService.GetById(workspace.Id));
        }

        [TestMethod]
        public void Delete_WhenWorkspaceNotExists_DoesNothing()
        {
            // Arrange
            var workspace = new Workspace { Title = "Test Workspace" };
            WorkspaceRepository.workspaceRepository.Add(workspace);

            // Act
            _workspaceService.Delete(Guid.NewGuid());

            // Assert
            Assert.AreEqual(1, WorkspaceRepository.workspaceRepository.Count());
        }
        #endregion

        #region GetJoinedWorkspaces
        [TestMethod]
        public void GetJoinedWorkspaces_WhenExists_ReturnsJoinedWorkspaces()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workspace1 = new Workspace { Title = "Workspace 1" };
            var workspace2 = new Workspace { Title = "Workspace 2" };
            var otherWorkspace = new Workspace { Title = "Other Workspace" };

            WorkspaceRepository.workspaceRepository.Add(workspace1);
            WorkspaceRepository.workspaceRepository.Add(workspace2);
            WorkspaceRepository.workspaceRepository.Add(otherWorkspace);

            UserWorkspaceRepository.AddUserWorkspace(new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspace1.Id
            });
            UserWorkspaceRepository.AddUserWorkspace(new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspace2.Id
            });


            // Act
            var result = _workspaceService.GetJoinedWorkspaces(userId).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, workspace1);
            CollectionAssert.Contains(result, workspace2);
            CollectionAssert.DoesNotContain(result, otherWorkspace);
        }

        [TestMethod]
        public void GetJoinedWorkspaces_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = _workspaceService.GetJoinedWorkspaces(Guid.NewGuid());

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion
    }
}
