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
        private CitationService _citationService;

        [TestInitialize]
        public void Setup()
        {
            _citationService = new CitationService();
            // Clear the static repository before each test to ensure isolation
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
            // Verify ordering by CreatedAt (newest first)
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
                pages: "10-20",
                volume: "1",
                issue: "2",
                url: "http://example.com/book",
                accessURL: null,
                accessLocation: "Library",
                publicationInfo: "Book Series",
                publisher: "New Publisher",
                publicationDate: new DateTime(2023, 1, 1),
                publisherLocation: "New York",
                accessDate: "2024-01-01",
                doi: "10.1234/new.book",
                documentId: documentId
            );

            // Assert
            Assert.IsNotNull(newCitation);
            Assert.AreNotEqual(Guid.Empty, newCitation.Id);
            Assert.AreEqual(CitationType.Book, newCitation.Type);
            Assert.AreEqual("New Book Title", newCitation.Title);
            Assert.AreEqual("New Book Author", newCitation.Author);
            Assert.AreEqual("10-20", newCitation.Pages);
            Assert.AreEqual("1", newCitation.Volume);
            Assert.AreEqual("2", newCitation.Issue);
            Assert.AreEqual("http://example.com/book", newCitation.URL);
            Assert.IsNull(newCitation.AccessURL);
            Assert.AreEqual("Library", newCitation.AccessLocation);
            Assert.AreEqual("Book Series", newCitation.PublicationInfo);
            Assert.AreEqual("New Publisher", newCitation.Publisher);
            Assert.AreEqual(new DateTime(2023, 1, 1), newCitation.PublicationDate);
            Assert.AreEqual("New York", newCitation.PublisherLocation);
            Assert.AreEqual("2024-01-01", newCitation.AccessDate);
            Assert.AreEqual("10.1234/new.book", newCitation.DOI);
            Assert.AreEqual(documentId, newCitation.FK_DocumentId);
            Assert.AreEqual(initialCount + 1, CitationRepository.Citations.Count);
            Assert.IsNotNull(CitationRepository.GetCitationById(newCitation.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyTitle_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "", "Author", publicationInfo: "Pub Info", documentId: Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithNullAuthor_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "Title", null, publicationInfo: "Pub Info", documentId: Guid.NewGuid());
        }

        // Removed CreateCitation_WithWhitespacePublicationInfo_ThrowsArgumentException
        // as PublicationInfo is now optional and can be null/whitespace.

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyDocumentId_ThrowsArgumentException()
        {
            // Now documentId is nullable, so we test with both null and Guid.Empty
            _citationService.CreateCitation(CitationType.Book, "Title", "Author", documentId: Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithNullDocumentId_ThrowsArgumentException()
        {
            _citationService.CreateCitation(CitationType.Book, "Title", "Author", documentId: null);
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
                Pages = "1-5",
                Volume = "V1",
                Issue = "I1",
                URL = "http://original.com",
                AccessURL = null,
                AccessLocation = "Old Place",
                PublicationInfo = "Original Pub Info",
                Publisher = "Original Publisher",
                PublicationDate = new DateTime(2020, 1, 1),
                PublisherLocation = "Old City",
                AccessDate = "2020-01-01",
                DOI = "original.doi",
                FK_DocumentId = documentId,
                CreatedAt = DateTime.Now.AddDays(-10) // Set an older CreatedAt for UpdatedAt comparison
            };
            CitationRepository.AddCitation(existingCitation);

            // Act
            var updatedCitation = _citationService.UpdateCitation(
                existingCitation.Id,
                CitationType.JournalArticle,
                "Updated Title",
                "Updated Author",
                pages: "6-10",
                volume: "V2",
                issue: "I2",
                url: null,
                accessURL: "http://updated.com",
                accessLocation: "New Place",
                publicationInfo: "Updated Pub Info",
                publisher: "Updated Publisher",
                publicationDate: new DateTime(2022, 2, 2),
                publisherLocation: "New City",
                accessDate: "2022-02-02",
                doi: "updated.doi"
            );

            // Assert
            Assert.IsNotNull(updatedCitation);
            Assert.AreEqual(existingCitation.Id, updatedCitation.Id); // ID should remain the same
            Assert.AreEqual(CitationType.JournalArticle, updatedCitation.Type);
            Assert.AreEqual("Updated Title", updatedCitation.Title);
            Assert.AreEqual("Updated Author", updatedCitation.Author);
            Assert.AreEqual("6-10", updatedCitation.Pages);
            Assert.AreEqual("V2", updatedCitation.Volume);
            Assert.AreEqual("I2", updatedCitation.Issue);
            Assert.IsNull(updatedCitation.URL); // Changed to null
            Assert.AreEqual("http://updated.com", updatedCitation.AccessURL); // Changed from null
            Assert.AreEqual("New Place", updatedCitation.AccessLocation);
            Assert.AreEqual("Updated Pub Info", updatedCitation.PublicationInfo);
            Assert.AreEqual("Updated Publisher", updatedCitation.Publisher);
            Assert.AreEqual(new DateTime(2022, 2, 2), updatedCitation.PublicationDate);
            Assert.AreEqual("New City", updatedCitation.PublisherLocation);
            Assert.AreEqual("2022-02-02", updatedCitation.AccessDate);
            Assert.AreEqual("updated.doi", updatedCitation.DOI);
            Assert.IsTrue(updatedCitation.UpdatedAt > existingCitation.CreatedAt); // UpdatedAt should be newer
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
                publicationInfo: "Pub Info"
            );

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateCitation_WithEmptyGuid_ThrowsArgumentException()
        {
            _citationService.UpdateCitation(Guid.Empty, CitationType.Book, "Title", "Author", publicationInfo: "Pub Info");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateCitation_WithEmptyTitle_ThrowsArgumentException()
        {
            var existingCitation = new Citation { Type = CitationType.Book, Title = "Original", Author = "A", PublicationInfo = "P", FK_DocumentId = Guid.NewGuid() };
            CitationRepository.AddCitation(existingCitation);
            _citationService.UpdateCitation(existingCitation.Id, CitationType.Book, "", "Author", publicationInfo: "Pub Info");
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
                PublicationInfo = "Book Series",
                Publisher = "Publisher Inc.",
                PublisherLocation = "New York",
                FK_DocumentId = Guid.NewGuid(),
                PublicationDate = new DateTime(2020, 5, 10),
                URL = "http://example.com/book-online"
            };
            CitationRepository.AddCitation(citation);

            // Expected format based on the updated CitationFormatter code for Book:
            // "Author. (Year). *Title*. Publisher Location: Publisher. Diakses dari URL."
            var expectedFormat = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. {citation.PublisherLocation}: {citation.Publisher}. Diakses dari {citation.URL}.";


            // Act
            var result = _citationService.GetFormattedCitationAPA(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFormat, result);
        }

        [TestMethod]
        public void GetFormattedCitationAPA_Website_ReturnsFormattedString()
        {
            // Arrange
            var citation = new Citation
            {
                Type = CitationType.Website,
                Title = "Article on Website",
                Author = "Web Author, A.",
                PublicationInfo = "Example Site",
                AccessDate = "2024-05-23",
                URL = "http://example.com/article",
                FK_DocumentId = Guid.NewGuid(),
                PublicationDate = new DateTime(2023, 10, 15)
            };
            CitationRepository.AddCitation(citation);

            // Expected format based on the updated CitationFormatter code for Website:
            // "Author. (Year). *Title*. Site Name. Diakses dari URL. Diakses dari AccessDate."
            var expectedFormat = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. {citation.PublicationInfo}. Diakses dari {citation.URL}. Diakses dari {citation.AccessDate}.";

            // Act
            var result = _citationService.GetFormattedCitationAPA(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFormat, result);
        }

        [TestMethod]
        public void GetFormattedCitationAPA_JournalArticle_ReturnsFormattedString()
        {
            // Arrange
            var citation = new Citation
            {
                Type = CitationType.JournalArticle,
                Title = "Impact of AI",
                Author = "Smith, J.",
                PublicationInfo = "Journal of Technology",
                Volume = "15",
                Issue = "3",
                Pages = "112-125",
                DOI = "10.1000/xyz123",
                FK_DocumentId = Guid.NewGuid(),
                PublicationDate = new DateTime(2021, 7, 1)
            };
            CitationRepository.AddCitation(citation);

            // Expected format based on the updated CitationFormatter code for JournalArticle:
            // "Author. (Year). *Title*. *Title of Periodical, Volume*(Issue), pages. DOI"
            var expectedFormat = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. *{citation.PublicationInfo}*{citation.Volume}({citation.Issue}), {citation.Pages}. {citation.DOI}";

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
