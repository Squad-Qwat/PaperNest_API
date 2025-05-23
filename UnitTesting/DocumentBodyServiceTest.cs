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
    public class DocumentBodyServiceTests
    {
        private DocumentBodyService _documentBodyService;

        [TestInitialize]
        public void Setup()
        {
            _documentBodyService = new DocumentBodyService();
            
            DocumentBodyRepository.ClearAllDocumentBodies();
        }

        #region GetAllDocumentBodies
        [TestMethod]
        public void GetAllDocumentBodies_ReturnsAllDocumentBodies()
        {
            
            var documentBody1 = new DocumentBody { FK_DocumentId = Guid.NewGuid(), Content = "Content 1" };
            var documentBody2 = new DocumentBody { FK_DocumentId = Guid.NewGuid(), Content = "Content 2" };
            DocumentBodyRepository.AddDocumentBody(documentBody1);
            DocumentBodyRepository.AddDocumentBody(documentBody2);

            
            var result = _documentBodyService.GetAllDocumentBodies();

            
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, documentBody1);
            CollectionAssert.Contains(result, documentBody2);
        }

        [TestMethod]
        public void GetAllDocumentBodies_WhenEmpty_ReturnsEmptyList()
        {
            
            var result = _documentBodyService.GetAllDocumentBodies();

            
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region GetDocumentBodyById
        [TestMethod]
        public void GetDocumentBodyById_WhenExists_ReturnsDocumentBody()
        {
            
            var documentBody = new DocumentBody { Content = "Test Content" };
            DocumentBodyRepository.AddDocumentBody(documentBody);
            var documentBodyId = documentBody.Id;
            var documentId = documentBody.FK_DocumentId;


            var result = _documentBodyService.GetDocumentBodyById(documentId,documentBodyId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(documentBodyId, result.Id);
            Assert.AreEqual("Test Content", result.Content);
        }

        [TestMethod]
        public void GetDocumentBodyById_WhenNotExists_ReturnsNull()
        {
            
            var nonExistentDocumentId = Guid.NewGuid();
            var nonExistentDocumentBodyId = Guid.NewGuid();


            var result = _documentBodyService.GetDocumentBodyById(nonExistentDocumentId,nonExistentDocumentBodyId);

            
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDocumentBodyById_WithEmptyGuid_ThrowsArgumentException()
        {
            
            _documentBodyService.GetDocumentBodyById(Guid.Empty,Guid.Empty);
        }
        #endregion

        #region GetDocumentBodiesByDocumentId
        [TestMethod]
        public void GetDocumentBodiesByDocumentId_WhenExists_ReturnsDocumentBodies()
        {
            
            var documentId = Guid.NewGuid();
            var documentBody1 = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Content 1",
                IsCurrentVersion = true
            };
            var documentBody2 = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Content 2",
                IsCurrentVersion = false
            };
            var otherDocumentBody = new DocumentBody
            {
                FK_DocumentId = Guid.NewGuid(),
                Content = "Other Content"
            };

            DocumentBodyRepository.AddDocumentBody(documentBody1);
            DocumentBodyRepository.AddDocumentBody(documentBody2);
            DocumentBodyRepository.AddDocumentBody(otherDocumentBody);

            
            var result = _documentBodyService.GetDocumentBodiesByDocumentId(documentId).ToList();

            
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(documentBody1));
            Assert.IsTrue(result.Contains(documentBody2));
            Assert.IsFalse(result.Contains(otherDocumentBody));
        }

        [TestMethod]
        public void GetDocumentBodiesByDocumentId_WhenNoneExist_ReturnsEmptyList()
        {
            
            var documentId = Guid.NewGuid();
            var otherDocumentBody = new DocumentBody { FK_DocumentId = Guid.NewGuid(), Content = "Other Content" };
            DocumentBodyRepository.AddDocumentBody(otherDocumentBody);

            
            var result = _documentBodyService.GetDocumentBodiesByDocumentId(documentId).ToList();

            
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDocumentBodiesByDocumentId_WithEmptyGuid_ThrowsArgumentException()
        {
            
            _documentBodyService.GetDocumentBodiesByDocumentId(Guid.Empty);
        }
        #endregion

        #region CreateDocumentBody
        [TestMethod]
        public void CreateDocumentBody_CreatesNewDocumentBody()
        {
            
            var documentId = Guid.NewGuid();
            var userCreatorId = Guid.NewGuid();
            var comment = "New Comment";
            var content = "New Content";

            var result = _documentBodyService.CreateDocumentBody(documentId,userCreatorId, comment ,content);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(documentId, result.FK_DocumentId);
            Assert.AreEqual(content, result.Content);
            Assert.IsTrue(result.IsCurrentVersion);
            Assert.AreEqual(1, DocumentBodyRepository.DocumentBodies.Count);
        }

        [TestMethod]
        public void CreateDocumentBody_WithExistingCurrentVersion_UpdatesPreviousVersion()
        {
            
            var documentId = Guid.NewGuid();
            var userCreatorId = Guid.NewGuid();
            var existingDocumentBody = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Old Content",
                IsCurrentVersion = true
            };
            DocumentBodyRepository.AddDocumentBody(existingDocumentBody);

            
            var result = _documentBodyService.CreateDocumentBody(documentId, userCreatorId, "New Comment" ,"New Content");

            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCurrentVersion);
            Assert.IsFalse(existingDocumentBody.IsCurrentVersion);
            Assert.AreEqual(2, DocumentBodyRepository.DocumentBodies.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDocumentBody_WithEmptyContent_ThrowsArgumentException()
        {
            
            _documentBodyService.CreateDocumentBody(Guid.NewGuid(), Guid.NewGuid() , "" ,"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDocumentBody_WithNullContent_ThrowsArgumentException()
        {
            
            _documentBodyService.CreateDocumentBody(Guid.NewGuid(), Guid.NewGuid() , null ,null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDocumentBody_WithWhitespaceContent_ThrowsArgumentException()
        {
            
            _documentBodyService.CreateDocumentBody(Guid.NewGuid(), Guid.NewGuid() , "   " ,"   ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDocumentBody_WithEmptyGuid_ThrowsArgumentException()
        {
            
            _documentBodyService.CreateDocumentBody(Guid.Empty, Guid.Empty, "Comment" ,"Content");
        }
        #endregion

        #region GetCurrentVersion
        [TestMethod]
        public void GetCurrentVersion_WhenExists_ReturnsCurrentVersion()
        {
            
            var documentId = Guid.NewGuid();
            var currentVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Current Content",
                IsCurrentVersion = true
            };
            var oldVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Old Content",
                IsCurrentVersion = false
            };
            DocumentBodyRepository.AddDocumentBody(currentVersion);
            DocumentBodyRepository.AddDocumentBody(oldVersion);

            
            var result = _documentBodyService.GetCurrentVersion(documentId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(currentVersion, result);
            Assert.IsTrue(result.IsCurrentVersion);
            Assert.AreEqual("Current Content", result.Content);
        }

        [TestMethod]
        public void GetCurrentVersion_WhenNoCurrentVersion_ReturnsNull()
        {
            
            var documentId = Guid.NewGuid();
            var nonCurrentVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Non-current Content",
                IsCurrentVersion = false
            };
            DocumentBodyRepository.AddDocumentBody(nonCurrentVersion);

            
            var result = _documentBodyService.GetCurrentVersion(documentId);

            
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCurrentVersion_WhenNoDocumentBodies_ReturnsNull()
        {
            
            var documentId = Guid.NewGuid();

            
            var result = _documentBodyService.GetCurrentVersion(documentId);

            
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCurrentVersion_WithEmptyGuid_ThrowsArgumentException()
        {
            
            _documentBodyService.GetCurrentVersion(Guid.Empty);
        }

        [TestMethod]
        public void RemoveDocumentBody_WhenExists_ReturnsTrue()
        {
            var documentBody = new DocumentBody { Content = "Test Content", FK_DocumentId = Guid.NewGuid() };
            DocumentBodyRepository.AddDocumentBody(documentBody);
            var documentBodyId = documentBody.Id;
            var documentId = documentBody.FK_DocumentId;

            var result = _documentBodyService.RemoveDocumentBody(documentId, documentBodyId);

            Assert.IsTrue(result);
            Assert.AreEqual(0, DocumentBodyRepository.DocumentBodies.Count);
        }

        [TestMethod]
        public void RemoveDocumentBody_WhenNotExists_ReturnsFalse()
        {
            var documentBodyId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var result = _documentBodyService.RemoveDocumentBody(documentId, documentBodyId);
            Assert.IsFalse(result);
            Assert.AreEqual(0, DocumentBodyRepository.DocumentBodies.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveDocumentBody_WithEmptyGuid_ThrowsArgumentException()
        {
            _documentBodyService.RemoveDocumentBody(Guid.Empty, Guid.Empty);
        }

        [TestMethod]
        public void RemoveDocumentBody_WithNonExistentDocumentId_ReturnsFalse()
        {
            var documentBody = new DocumentBody { Content = "Test Content", FK_DocumentId = Guid.NewGuid() };
            DocumentBodyRepository.AddDocumentBody(documentBody);
            var documentBodyId = documentBody.Id;
            var documentId = Guid.NewGuid();
            var result = _documentBodyService.RemoveDocumentBody(documentId, documentBodyId);
            Assert.IsFalse(result);
            Assert.AreEqual(1, DocumentBodyRepository.DocumentBodies.Count);
        }

        [TestMethod]
        public void RemoveDocumentBody_WithNonExistentDocumentBodyId_ReturnsFalse()
        {
            var documentBody = new DocumentBody { Content = "Test Content", FK_DocumentId = Guid.NewGuid() };
            DocumentBodyRepository.AddDocumentBody(documentBody);
            var documentBodyId = Guid.NewGuid();
            var documentId = documentBody.FK_DocumentId;
            var result = _documentBodyService.RemoveDocumentBody(documentId, documentBodyId);
            Assert.IsFalse(result);
            Assert.AreEqual(1, DocumentBodyRepository.DocumentBodies.Count);
        }

        [TestMethod]
        public void CanCreateNewVersion_WhenCurrentVersionExists_AndIsReviewed_ReturnsTrue()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var currentVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Current Content",
                IsCurrentVersion = true,
                IsReviewed = true // Versi sudah direview
            };
            DocumentBodyRepository.AddDocumentBody(currentVersion);
            
            // Act
            var result = _documentBodyService.CanCreateNewVersion(documentId);
            
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanCreateNewVersion_WhenCurrentVersionExists_AndNotReviewed_ReturnsFalse()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var currentVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Current Content",
                IsCurrentVersion = true,
                IsReviewed = false // Versi belum direview
            };
            DocumentBodyRepository.AddDocumentBody(currentVersion);
            
            // Act
            var result = _documentBodyService.CanCreateNewVersion(documentId);
            
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanCreateNewVersion_WhenNoVersion_ReturnsTrue()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            
            // Act
            var result = _documentBodyService.CanCreateNewVersion(documentId);
            
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RollbackToPreviousDocumentBody_WhenPreviousVersionExists_ReturnsDocumentBody()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var previousVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Previous Content",
                IsCurrentVersion = false
            };
            var currentVersion = new DocumentBody
            {
                FK_DocumentId = documentId,
                Content = "Current Content",
                IsCurrentVersion = true
            };
            DocumentBodyRepository.AddDocumentBody(previousVersion);
            DocumentBodyRepository.AddDocumentBody(currentVersion);
            
            // Act
            var result = _documentBodyService.RollbackToPreviousDocumentBody(documentId, previousVersion.Id);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(previousVersion.Id, result.Id);
            Assert.IsTrue(result.IsCurrentVersion);
            Assert.IsFalse(currentVersion.IsCurrentVersion);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RollbackToPreviousDocumentBody_WhenNoPreviousVersion_ThrowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            // Tidak ada versi yang ditambahkan ke DocumentBodyRepository
            
            // Act - Expect exception dengan pesan "Document body not found"
            _documentBodyService.RollbackToPreviousDocumentBody(documentId, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RollbackToPreviousDocumentBody_WhenPreviousVersionNotExists_ThrowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var nonExistentDocumentBodyId = Guid.NewGuid();
            
            // Act - Expect exception dengan pesan "Document body not found"
            _documentBodyService.RollbackToPreviousDocumentBody(documentId, nonExistentDocumentBodyId);
        }

        #endregion
    }
}
