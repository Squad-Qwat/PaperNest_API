using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using API.Services;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class WorkspaceServiceTests
    {
        [TestInitialize]
        public void Setup()
        {
            WorkspaceRepository.workspaceRepository.Clear();
            UserRepository.userWorkspaceRepository.Clear();
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
            WorkspaceService.Create(workspace);

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
            var result = WorkspaceService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            CollectionAssert.Contains(result.ToList(), workspace1);
            CollectionAssert.Contains(result.ToList(), workspace2);
        }

        [TestMethod]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = WorkspaceService.GetAll();

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
            var result = WorkspaceService.GetById(workspace.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(workspace.Id, result.Id);
            Assert.AreEqual(workspace.Title, result.Title);
        }

        [TestMethod]
        public void GetById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = WorkspaceService.GetById(Guid.NewGuid());

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
            workspace1.UserWorkspaces = new List<UserWorkspace>
            {
                new UserWorkspace { FK_UserId = userId }
            };
            var workspace2 = new Workspace { Title = "Workspace 2" };
            workspace2.UserWorkspaces = new List<UserWorkspace>
            {
                new UserWorkspace { FK_UserId = userId }
            };
            var otherWorkspace = new Workspace { Title = "Other Workspace" };

            WorkspaceRepository.workspaceRepository.Add(workspace1);
            WorkspaceRepository.workspaceRepository.Add(workspace2);
            WorkspaceRepository.workspaceRepository.Add(otherWorkspace);

            // Act
            var result = WorkspaceService.GetByUserId(userId).ToList();

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
            var result = WorkspaceService.GetByUserId(Guid.NewGuid());

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
            var result = WorkspaceService.JoinWorkspace(workspace.Id, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.FK_UserId);
            Assert.AreEqual(workspace.Id, result.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Lecturer, result.WorkspaceRole);
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
            var result = WorkspaceService.JoinWorkspace(workspace.Id, userId);

            // Assert
            Assert.AreEqual(existingUserWorkspace, result);
        }

        [TestMethod]
        public void JoinWorkspace_WhenWorkspaceNotExists_ReturnsNull()
        {
            // Act
            var result = WorkspaceService.JoinWorkspace(Guid.NewGuid(), Guid.NewGuid());

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
            WorkspaceService.Update(existingWorkspace.Id, updatedWorkspace);

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
            WorkspaceService.Update(Guid.NewGuid(), updatedWorkspace);

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
            WorkspaceService.Delete(workspace.Id);

            // Assert
            Assert.AreEqual(0, WorkspaceRepository.workspaceRepository.Count());
            Assert.IsNull(WorkspaceService.GetById(workspace.Id));
        }

        [TestMethod]
        public void Delete_WhenWorkspaceNotExists_DoesNothing()
        {
            // Arrange
            var workspace = new Workspace { Title = "Test Workspace" };
            WorkspaceRepository.workspaceRepository.Add(workspace);

            // Act
            WorkspaceService.Delete(Guid.NewGuid());

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

            UserRepository.userWorkspaceRepository.Add(new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspace1.Id
            });
            UserRepository.userWorkspaceRepository.Add(new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspace2.Id
            });

            // Act
            var result = WorkspaceService.GetJoinedWorkspaces(userId).ToList();

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
            var result = WorkspaceService.GetJoinedWorkspaces(Guid.NewGuid());

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion
    }
}