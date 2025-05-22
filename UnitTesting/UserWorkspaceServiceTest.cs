using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class UserWorkspaceServiceTests
    {
        private UserWorkspaceService _userWorkspaceService;

        [TestInitialize]
        public void Setup()
        {
            _userWorkspaceService = new UserWorkspaceService();

            UserWorkspaceRepository.UserWorkspace = new List<UserWorkspace>();
        }

        #region GetAllUserWorkspaces
        [TestMethod]
        public void GetAllUserWorkspaces_ReturnsAllUserWorkspaces()
        {
            
            var userWorkspace1 = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Owner
            };
            var userWorkspace2 = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };
            UserWorkspaceRepository.UserWorkspace.Add(userWorkspace1);
            UserWorkspaceRepository.UserWorkspace.Add(userWorkspace2);

            
            var result = _userWorkspaceService.GetAllUserWorkspaces();

            
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, userWorkspace1);
            CollectionAssert.Contains(result, userWorkspace2);
        }

        [TestMethod]
        public void GetAllUserWorkspaces_WhenEmpty_ReturnsEmptyList()
        {
            
            var result = _userWorkspaceService.GetAllUserWorkspaces();

            
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region GetUserWorkspaceById
        [TestMethod]
        public void GetUserWorkspaceById_WhenExists_ReturnsUserWorkspace()
        {
            
            var userWorkspace = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Lecturer
            };
            UserWorkspaceRepository.UserWorkspace.Add(userWorkspace);
            var userWorkspaceId = userWorkspace.Id;

            
            var result = _userWorkspaceService.GetUserWorkspaceById(userWorkspaceId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(userWorkspaceId, result.Id);
            Assert.AreEqual(WorkspaceRole.Lecturer, result.WorkspaceRole);
        }

        [TestMethod]
        public void GetUserWorkspaceById_WhenNotExists_ReturnsNull()
        {
            
            var nonExistentUserWorkspaceId = Guid.NewGuid();

            
            var result = _userWorkspaceService.GetUserWorkspaceById(nonExistentUserWorkspaceId);

            
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
            
            var userId = Guid.NewGuid();
            var userWorkspace1 = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Owner
            };
            var userWorkspace2 = new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };
            var otherUserWorkspace = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };

            UserWorkspaceRepository.UserWorkspace.Add(userWorkspace1);
            UserWorkspaceRepository.UserWorkspace.Add(userWorkspace2);
            UserWorkspaceRepository.UserWorkspace.Add(otherUserWorkspace);

            
            var result = _userWorkspaceService.GetUserWorkspacesByUserId(userId).ToList();

            
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(userWorkspace1));
            Assert.IsTrue(result.Contains(userWorkspace2));
            Assert.IsFalse(result.Contains(otherUserWorkspace));
        }

        [TestMethod]
        public void GetUserWorkspacesByUserId_WhenNoneExist_ReturnsEmptyList()
        {
            
            var userId = Guid.NewGuid();
            var otherUserWorkspace = new UserWorkspace
            {
                FK_UserId = Guid.NewGuid(),
                FK_WorkspaceId = Guid.NewGuid(),
                WorkspaceRole = WorkspaceRole.Member
            };
            UserWorkspaceRepository.UserWorkspace.Add(otherUserWorkspace);

            
            var result = _userWorkspaceService.GetUserWorkspacesByUserId(userId).ToList();

            
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
        public void AddUserWorkspaceAsOwner_AddsNewUserWorkspace()
        {
            
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            
            _userWorkspaceService.AddUserWorkspaceAsOwner(userId, workspaceId);

            
            Assert.AreEqual(1, UserWorkspaceRepository.UserWorkspace.Count);
            var addedUserWorkspace = UserWorkspaceRepository.UserWorkspace.First();
            Assert.AreEqual(userId, addedUserWorkspace.FK_UserId);
            Assert.AreEqual(workspaceId, addedUserWorkspace.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Owner, addedUserWorkspace.WorkspaceRole);
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
        public void AddUserWorkspaceAsMember_AddsNewUserWorkspace()
        {
            
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            
            _userWorkspaceService.AddUserWorkspaceAsMember(userId, workspaceId);

            
            Assert.AreEqual(1, UserWorkspaceRepository.UserWorkspace.Count);
            var addedUserWorkspace = UserWorkspaceRepository.UserWorkspace.First();
            Assert.AreEqual(userId, addedUserWorkspace.FK_UserId);
            Assert.AreEqual(workspaceId, addedUserWorkspace.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Member, addedUserWorkspace.WorkspaceRole);
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
        public void AddUserWorkspaceAsLecturer_AddsNewUserWorkspace()
        {
            
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            
            _userWorkspaceService.AddUserWorkspaceAsLecturer(userId, workspaceId);

            
            Assert.AreEqual(1, UserWorkspaceRepository.UserWorkspace.Count);
            var addedUserWorkspace = UserWorkspaceRepository.UserWorkspace.First();
            Assert.AreEqual(userId, addedUserWorkspace.FK_UserId);
            Assert.AreEqual(workspaceId, addedUserWorkspace.FK_WorkspaceId);
            Assert.AreEqual(WorkspaceRole.Lecturer, addedUserWorkspace.WorkspaceRole);
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
        #endregion
    }
}
