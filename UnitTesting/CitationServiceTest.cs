using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;
using API.Services;
using API.StateMachines;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize] // Ensures tests run sequentially, important for static repositories
    public class CitationServiceTests
    {
        private CitationService? _citationService;

        [TestInitialize]
        public void Setup()
        {
            _citationService = new CitationService();
            CitationRepository.ClearAllCitations();
        }

        #region GetAllCitations
        [TestMethod]
        public void GetAllCitations_ReturnsAllCitations()
        {
            // Arrange
            var citation1 = new Citation { Type = CitationType.Book, Title = "Title1", Author = "Author1", PublicationInfo = "Pub1", FK_DocumentId = Guid.NewGuid() };
            var citation2 = new Citation { Type = CitationType.Website, Title = "Title2", Author = "Author2", PublicationInfo = "Pub2", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(citation1);
            CitationRepository.AddCitation(citation2);

            // Act
            var result = _citationService.GetAllCitations();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, citation1);
            CollectionAssert.Contains(result, citation2);
        }

        [TestMethod]
        public void GetAllCitations_WhenEmpty_ReturnsEmptyList()
        {
            // Arrange (repository is already cleared in Setup)

            // Act
            var result = _citationService.GetAllCitations();

            // Assert
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region GetCitationById
        [TestMethod]
        public void GetCitationById_WhenExists_ReturnsCitation()
        {
            // Arrange
            var citation = new Citation { Type = CitationType.Book, Title = "Test Title", Author = "Test Author", PublicationInfo = "Test Pub", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(citation);

            // Act
            var result = _citationService.GetCitationById(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(citation.Id, result.Id);
            Assert.AreEqual("Test Title", result.Title);
        }

        [TestMethod]
        public void GetCitationById_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _citationService.GetCitationById(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCitationById_WithEmptyGuid_ThrowsArgumentException()
        {
            // Act
            _citationService.GetCitationById(Guid.Empty);
        }
        #endregion

        #region GetCitationsByDocumentId
        [TestMethod]
        public void GetCitationsByDocumentId_WhenExists_ReturnsCitations()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var citation1 = new Citation { Type = CitationType.Book, Title = "Doc1 Cit1", Author = "A", PublicationInfo = "P", FK_DocumentId = documentId, CreatedAt = DateTime.Now.AddDays(-2) };
            var citation2 = new Citation { Type = CitationType.Website, Title = "Doc1 Cit2", Author = "B", PublicationInfo = "Q", FK_DocumentId = documentId, CreatedAt = DateTime.Now.AddDays(-1) };
            var otherCitation = new Citation { Type = CitationType.JournalArticle, Title = "Other Doc Cit", Author = "C", PublicationInfo = "R", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(citation1);
            CitationRepository.AddCitation(citation2);
            CitationRepository.AddCitation(otherCitation);

            // Act
            var result = _citationService.GetCitationsByDocumentId(documentId).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(citation1));
            Assert.IsTrue(result.Contains(citation2));
            Assert.IsFalse(result.Contains(otherCitation));
            // Verify ordering by Created_at (newest first)
            Assert.AreEqual(citation2.Id, result[0].Id);
            Assert.AreEqual(citation1.Id, result[1].Id);
        }

        [TestMethod]
        public void GetCitationsByDocumentId_WhenNoneExist_ReturnsEmptyList()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var otherCitation = new Citation { Type = CitationType.Book, Title = "Other Cit", Author = "X", PublicationInfo = "Y", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(otherCitation);

            // Act
            var result = _citationService.GetCitationsByDocumentId(documentId).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCitationsByDocumentId_WithEmptyGuid_ThrowsArgumentException()
        {
            // Act
            _citationService.GetCitationsByDocumentId(Guid.Empty);
        }
        #endregion

        #region CreateCitation
        [TestMethod]
        public void CreateCitation_CreatesNewCitation()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var initialCount = CitationRepository.Citations.Count;

            // Act
            var newCitation = _citationService.CreateCitation(
                CitationType.Book,
                "New Book Title",
                "New Book Author",
                "New Book Pub Info",
                documentId,
                new DateTime(2023, 1, 1),
                null,
                "10.1234/new.book"
            );

            // Assert
            Assert.IsNotNull(newCitation);
            Assert.AreNotEqual(Guid.Empty, newCitation.Id);
            Assert.AreEqual(CitationType.Book, newCitation.Type);
            Assert.AreEqual("New Book Title", newCitation.Title);
            Assert.AreEqual("New Book Author", newCitation.Author);
            Assert.AreEqual("New Book Pub Info", newCitation.PublicationInfo);
            Assert.AreEqual(documentId, newCitation.FK_DocumentId);
            Assert.AreEqual(new DateTime(2023, 1, 1), newCitation.PublicationDate);
            Assert.IsNull(newCitation.AccessDate);
            Assert.AreEqual("10.1234/new.book", newCitation.DOI);
            Assert.AreEqual(initialCount + 1, CitationRepository.Citations.Count);
            Assert.IsNotNull(CitationRepository.GetCitationById(newCitation.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyTitle_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "", "Author", "Pub Info", Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithNullAuthor_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "Title", null, "Pub Info", Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithWhitespacePublicationInfo_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "Title", "Author", "   ", Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyDocumentId_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "Title", "Author", "Pub Info", Guid.Empty);
        }
        #endregion

        #region UpdateCitation
        [TestMethod]
        public void UpdateCitation_WhenExists_UpdatesCitation()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var existingCitation = new Citation
            {
                Type = CitationType.Book,
                Title = "Original Title",
                Author = "Original Author",
                PublicationInfo = "Original Pub Info",
                FK_DocumentId = documentId,
                PublicationDate = new DateTime(2020, 1, 1),
                AccessDate = "2020-01-01",
                DOI = "original.doi"
            };
            CitationRepository.AddCitation(existingCitation);

            // Act
            var updatedCitation = _citationService.UpdateCitation(
                existingCitation.Id,
                CitationType.JournalArticle,
                "Updated Title",
                "Updated Author",
                "Updated Pub Info",
                new DateTime(2022, 2, 2),
                "2022-02-02",
                "updated.doi"
            );

            // Assert
            Assert.IsNotNull(updatedCitation);
            Assert.AreEqual(existingCitation.Id, updatedCitation.Id); // ID should remain the same
            Assert.AreEqual(CitationType.JournalArticle, updatedCitation.Type);
            Assert.AreEqual("Updated Title", updatedCitation.Title);
            Assert.AreEqual("Updated Author", updatedCitation.Author);
            Assert.AreEqual("Updated Pub Info", updatedCitation.PublicationInfo);
            Assert.AreEqual(new DateTime(2022, 2, 2), updatedCitation.PublicationDate);
            Assert.AreEqual("2022-02-02", updatedCitation.AccessDate);
            Assert.AreEqual("updated.doi", updatedCitation.DOI);
            Assert.IsTrue(updatedCitation.UpdatedAt > existingCitation.CreatedAt); // Updated_at should be newer
        }

        [TestMethod]
        public void UpdateCitation_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _citationService.UpdateCitation(
                nonExistentId,
                CitationType.Book,
                "Title",
                "Author",
                "Pub Info",
                null, null, null
            );

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateCitation_WithEmptyGuid_ThrowsArgumentException()
        {
            _citationService.UpdateCitation(Guid.Empty, CitationType.Book, "Title", "Author", "Pub Info");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateCitation_WithEmptyTitle_ThrowsArgumentException()
        {
            var existingCitation = new Citation { Type = CitationType.Book, Title = "Original", Author = "A", PublicationInfo = "P", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(existingCitation);
            _citationService.UpdateCitation(existingCitation.Id, CitationType.Book, "", "Author", "Pub Info");
        }
        #endregion

        #region DeleteCitation
        [TestMethod]
        public void DeleteCitation_WhenExists_ReturnsTrue()
        {
            // Arrange
            var citation = new Citation { Type = CitationType.Book, Title = "Test Title", Author = "Test Author", PublicationInfo = "Test Pub", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(citation);
            var initialCount = CitationRepository.Citations.Count;

            // Act
            var result = _citationService.DeleteCitation(citation.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(initialCount - 1, CitationRepository.Citations.Count);
            Assert.IsNull(CitationRepository.GetCitationById(citation.Id));
        }

        [TestMethod]
        public void DeleteCitation_WhenNotExists_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var initialCount = CitationRepository.Citations.Count;

            // Act
            var result = _citationService.DeleteCitation(nonExistentId);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(initialCount, CitationRepository.Citations.Count); // Count should remain unchanged
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteCitation_WithEmptyGuid_ThrowsArgumentException()
        {
            // Act
            _citationService.DeleteCitation(Guid.Empty);
        }
        #endregion

        #region GetFormattedCitationAPA
        [TestMethod]
        public void GetFormattedCitationAPA_WhenExists_ReturnsFormattedString()
        {
            // Arrange
            var citation = new Citation
            {
                Type = CitationType.Book,
                Title = "The Great Book",
                Author = "Doe, J.",
                PublicationInfo = "Publisher Inc.",
                FK_DocumentId = Guid.NewGuid(),
                PublicationDate = new DateTime(2020, 5, 10)
            };
            CitationRepository.AddCitation(citation);

            // Mock CitationFormatter.GenerateAPAStyle to return a predictable string
            // For actual unit testing, you might mock the static method or use a wrapper.
            // For simplicity here, we'll assume CitationFormatter works as expected and test the service's call to it.
            // Expected format based on the provided CitationFormatter code for Book:
            var expectedFormat = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. {citation.PublicationInfo}.";


            // Act
            var result = _citationService.GetFormattedCitationAPA(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFormat, result);
        }

        [TestMethod]
        public void GetFormattedCitationAPA_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _citationService.GetFormattedCitationAPA(nonExistentId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFormattedCitationAPA_WithEmptyGuid_ThrowsArgumentException()
        {
            // Act
            _citationService.GetFormattedCitationAPA(Guid.Empty);
        }
        #endregion
    }
}
