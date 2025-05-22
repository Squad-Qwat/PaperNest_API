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
    public class DocumentServiceTests
    {
        private DocumentService? _documentService;

        [TestInitialize]
        public void Setup()
        {
            _documentService = new DocumentService();
            DocumentRepository.documentRepository.Clear();
            UserWorkspaceRepository.UserWorkspace.Clear();
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
            _documentService.Create(document);

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
            var result = _documentService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            CollectionAssert.Contains(result.ToList(), document1);
            CollectionAssert.Contains(result.ToList(), document2);
        }

        [TestMethod]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _documentService.GetAll();

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
            var result = _documentService.GetById(document.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(document.Id, result.Id);
            Assert.AreEqual(document.Title, result.Title);
        }

        [TestMethod]
        public void GetById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = _documentService.GetById(Guid.NewGuid());

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
            var workspaceId = Guid.NewGuid();

            // Create documents with the workspace ID
            var document1 = new Document
            {
                Title = "Doc 1",
                FK_WorkspaceId = workspaceId
            };
            var document2 = new Document
            {
                Title = "Doc 2",
                FK_WorkspaceId = workspaceId
            };
            var otherDocument = new Document
            {
                Title = "Other Doc",
                FK_WorkspaceId = Guid.NewGuid() // Different workspace
            };

            // Add documents to repository
            DocumentRepository.documentRepository.Add(document1);
            DocumentRepository.documentRepository.Add(document2);
            DocumentRepository.documentRepository.Add(otherDocument);

            // Create a user-workspace relationship
            UserWorkspaceRepository.AddUserWorkspace(new UserWorkspace
            {
                FK_UserId = userId,
                FK_WorkspaceId = workspaceId
            });

            // Act - Run the method we're testing
            var result = _documentService.GetByUserId(userId).ToList();

            // Assert - Check the results match our expectations
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(document1));
            Assert.IsTrue(result.Contains(document2));
            Assert.IsFalse(result.Contains(otherDocument));
        }

        [TestMethod]
        public void GetByUserId_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = _documentService.GetByUserId(Guid.NewGuid());

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

            // Buat dokumen yang terkait dengan workspaceId
            var document1 = new Document
            {
                Title = "Doc 1",
                FK_WorkspaceId = workspaceId
            };
            var document2 = new Document
            {
                Title = "Doc 2",
                FK_WorkspaceId = workspaceId
            };
            var otherDocument = new Document
            {
                Title = "Other Doc",
                FK_WorkspaceId = Guid.NewGuid()
            };

            DocumentRepository.documentRepository.Add(document1);
            DocumentRepository.documentRepository.Add(document2);
            DocumentRepository.documentRepository.Add(otherDocument);

            // Act
            var result = _documentService.GetByWorkspaceId(workspaceId).ToList();

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
            var result = _documentService.GetByWorkspaceId(Guid.NewGuid());

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
            _documentService.Update(existingDocument.Id, updatedDocument);

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
            _documentService.Update(nonExistentId, updatedDocument);

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
            _documentService.Delete(documentId);

            // Assert
            Assert.AreEqual(0, DocumentRepository.documentRepository.Count());
            Assert.IsNull(_documentService.GetById(documentId));
        }

        [TestMethod]
        public void Delete_WhenDocumentNotExists_DoesNothing()
        {
            // Arrange
            var document = new Document { Title = "Test Document" };
            DocumentRepository.documentRepository.Add(document);

            // Act
            _documentService.Delete(Guid.NewGuid());

            // Assert
            Assert.AreEqual(1, DocumentRepository.documentRepository.Count());
        }
        #endregion
    }
}
