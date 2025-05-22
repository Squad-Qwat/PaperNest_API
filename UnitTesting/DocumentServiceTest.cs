using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class DocumentServiceTests
    {
        [TestInitialize]
        public void Setup()
        {
            DocumentRepository.documentRepository.Clear();
        }

        #region Create
        [TestMethod]
        public void Create_AddsNewDocument()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                SavedContent = "Test Content",
                FK_WorkspaceId = Guid.NewGuid()
            };

            // Act
            DocumentService.Create(document);

            // Assert
            Assert.AreEqual(1, DocumentRepository.documentRepository.Count());
            Assert.IsTrue(DocumentRepository.documentRepository.Contains(document));
        }
        #endregion

        #region GetAll
        [TestMethod]
        public void GetAll_ReturnsAllDocuments()
        {
            // Arrange
            var document1 = new Document { Title = "Document 1", FK_WorkspaceId = Guid.NewGuid() };
            var document2 = new Document { Title = "Document 2", FK_WorkspaceId = Guid.NewGuid() };
            DocumentRepository.documentRepository.Add(document1);
            DocumentRepository.documentRepository.Add(document2);

            // Act
            var result = DocumentService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            CollectionAssert.Contains(result.ToList(), document1);
            CollectionAssert.Contains(result.ToList(), document2);
        }

        [TestMethod]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = DocumentService.GetAll();

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion

        #region GetById
        [TestMethod]
        public void GetById_WhenExists_ReturnsDocument()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                FK_WorkspaceId = Guid.NewGuid()
            };
            DocumentRepository.documentRepository.Add(document);

            // Act
            var result = DocumentService.GetById(document.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(document.Id, result.Id);
            Assert.AreEqual(document.Title, result.Title);
        }

        [TestMethod]
        public void GetById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = DocumentService.GetById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }
        #endregion

        #region GetByUserId
        [TestMethod]
        public void GetByUserId_WhenExists_ReturnsUserDocuments()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workspace = new Workspace { };
            var userWorkspace = new UserWorkspace { FK_UserId = userId, User = new User { } };
            workspace.UserWorkspaces = new List<UserWorkspace> { userWorkspace };

            var document1 = new Document { Title = "Doc 1", Workspace = workspace };
            var document2 = new Document { Title = "Doc 2", Workspace = workspace };
            var otherDocument = new Document { Title = "Other Doc", Workspace = new Workspace() };

            DocumentRepository.documentRepository.Add(document1);
            DocumentRepository.documentRepository.Add(document2);
            DocumentRepository.documentRepository.Add(otherDocument);

            // Act
            var result = DocumentService.GetByUserId(userId).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, document1);
            CollectionAssert.Contains(result, document2);
            CollectionAssert.DoesNotContain(result, otherDocument);
        }

        [TestMethod]
        public void GetByUserId_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = DocumentService.GetByUserId(Guid.NewGuid());

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion

        #region GetByWorkspaceId
        [TestMethod]
        public void GetByWorkspaceId_WhenExists_ReturnsWorkspaceDocuments()
        {
            // Arrange
            var workspaceId = Guid.NewGuid();
            var workspace = new Workspace { };
            var document1 = new Document { Workspace = workspace };
            var document2 = new Document { Workspace = workspace };
            var otherDocument = new Document { Workspace = new Workspace() };

            DocumentRepository.documentRepository.Add(document1);
            DocumentRepository.documentRepository.Add(document2);
            DocumentRepository.documentRepository.Add(otherDocument);

            // Act
            var result = DocumentService.GetByWorkspaceId(workspaceId).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, document1);
            CollectionAssert.Contains(result, document2);
            CollectionAssert.DoesNotContain(result, otherDocument);
        }

        [TestMethod]
        public void GetByWorkspaceId_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = DocumentService.GetByWorkspaceId(Guid.NewGuid());

            // Assert
            Assert.AreEqual(0, result.Count());
        }
        #endregion

        #region Update
        [TestMethod]
        public void Update_WhenDocumentExists_UpdatesDocument()
        {
            // Arrange
            var existingDocument = new Document
            {
                Title = "Old Title",
                SavedContent = "Old Content"
            };
            DocumentRepository.documentRepository.Add(existingDocument);

            var updatedDocument = new Document
            {
                Title = "New Title",
                SavedContent = "New Content"
            };

            // Act
            DocumentService.Update(existingDocument.Id, updatedDocument);

            // Assert
            Assert.AreEqual("New Title", existingDocument.Title);
            Assert.AreEqual("New Content", existingDocument.SavedContent);
            Assert.IsTrue(existingDocument.UpdateAt > DateTime.MinValue);
        }

        [TestMethod]
        public void Update_WhenDocumentNotExists_DoesNothing()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updatedDocument = new Document
            {
                Title = "New Title",
                SavedContent = "New Content"
            };

            // Act
            DocumentService.Update(nonExistentId, updatedDocument);

            // Assert
            Assert.AreEqual(0, DocumentRepository.documentRepository.Count());
        }
        #endregion

        #region Delete
        [TestMethod]
        public void Delete_WhenDocumentExists_RemovesDocument()
        {
            // Arrange
            var document = new Document { Title = "Test Document" };
            DocumentRepository.documentRepository.Add(document);
            var documentId = document.Id;

            // Act
            DocumentService.Delete(documentId);

            // Assert
            Assert.AreEqual(0, DocumentRepository.documentRepository.Count());
            Assert.IsNull(DocumentService.GetById(documentId));
        }

        [TestMethod]
        public void Delete_WhenDocumentNotExists_DoesNothing()
        {
            // Arrange
            var document = new Document { Title = "Test Document" };
            DocumentRepository.documentRepository.Add(document);

            // Act
            DocumentService.Delete(Guid.NewGuid());

            // Assert
            Assert.AreEqual(1, DocumentRepository.documentRepository.Count());
        }
        #endregion
    }
}