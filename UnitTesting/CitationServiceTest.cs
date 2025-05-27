using API.Helpers.Enums;
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
    public class CitationServiceTests
    {
        private CitationService? _citationService;
        private DocumentService? _documentService;

        [TestInitialize]
        public void Setup()
        {
            _citationService = new CitationService();
            _documentService = new DocumentService();
            CitationRepository.ClearAllCitations();
            DocumentRepository.documentRepository.Clear();
        }

        #region GetAllCitations
        [TestMethod]
        public void GetAllCitations_ReturnsAllCitations()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var citation1 = new Citation
            {
                Type = CitationType.Book,
                Title = "Citation 1",
                Author = "Author 1",
                PublicationInfo = "Publisher 1",
                FK_DocumentId = documentId
            };
            var citation2 = new Citation
            {
                Type = CitationType.JournalArticle,
                Title = "Citation 2",
                Author = "Author 2",
                PublicationInfo = "Journal 2",
                FK_DocumentId = documentId
            };
            CitationRepository.AddCitation(citation1);
            CitationRepository.AddCitation(citation2);

            // Act
            var result = _citationService?.GetAllCitations();

            // Assert
            Assert.AreEqual(2, result?.Count);
            CollectionAssert.Contains(result, citation1);
            CollectionAssert.Contains(result, citation2);
        }

        [TestMethod]
        public void GetAllCitations_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _citationService?.GetAllCitations();

            // Assert
            Assert.AreEqual(0, result?.Count);
        }
        #endregion

        #region GetCitationById
        [TestMethod]
        public void GetCitationById_WhenExists_ReturnsCitation()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var citation = new Citation
            {
                Type = CitationType.Book,
                Title = "Test Citation",
                Author = "Test Author",
                PublicationInfo = "Test Publisher",
                FK_DocumentId = documentId
            };
            CitationRepository.AddCitation(citation);

            // Act
            var result = _citationService?.GetCitationById(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(citation.Id, result.Id);
            Assert.AreEqual(citation.Title, result.Title);
            Assert.AreEqual(citation.Author, result.Author);
            Assert.AreEqual(citation.Type, result.Type);
        }

        [TestMethod]
        public void GetCitationById_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = _citationService?.GetCitationById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCitationById_WithEmptyGuid_ThrowsException()
        {
            // Act
            _citationService?.GetCitationById(Guid.Empty);
        }
        #endregion

        #region GetCitationsByDocumentId
        [TestMethod]
        public void GetCitationsByDocumentId_WhenExists_ReturnsCitations()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var anotherDocumentId = Guid.NewGuid();
            
            var citation1 = new Citation
            {
                Type = CitationType.Book,
                Title = "Citation 1",
                Author = "Author 1",
                PublicationInfo = "Publisher 1",
                FK_DocumentId = documentId
            };
            var citation2 = new Citation
            {
                Type = CitationType.JournalArticle,
                Title = "Citation 2",
                Author = "Author 2",
                PublicationInfo = "Journal 2",
                FK_DocumentId = documentId
            };
            var otherCitation = new Citation
            {
                Type = CitationType.Website,
                Title = "Other Citation",
                Author = "Other Author",
                PublicationInfo = "Website",
                FK_DocumentId = anotherDocumentId
            };
            
            CitationRepository.AddCitation(citation1);
            CitationRepository.AddCitation(citation2);
            CitationRepository.AddCitation(otherCitation);

            // Act
            var result = _citationService?.GetCitationsByDocumentId(documentId).ToList();

            // Assert
            Assert.AreEqual(2, result?.Count);
            CollectionAssert.Contains(result, citation1);
            CollectionAssert.Contains(result, citation2);
            CollectionAssert.DoesNotContain(result, otherCitation);
        }

        [TestMethod]
        public void GetCitationsByDocumentId_WhenNoneExist_ReturnsEmptyList()
        {
            // Act
            var result = _citationService?.GetCitationsByDocumentId(Guid.NewGuid());

            // Assert
            Assert.AreEqual(0, result?.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCitationsByDocumentId_WithEmptyGuid_ThrowsException()
        {
            // Act
            _citationService?.GetCitationsByDocumentId(Guid.Empty);
        }
        #endregion

        #region CreateCitation
        [TestMethod]
        public void CreateCitation_AddsNewCitation()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var type = CitationType.Book;
            var title = "Test Book";
            var author = "Test Author";
            var publicationInfo = "Test Publisher";

            // Act
            var citation = _citationService?.CreateCitation(
                type, 
                title, 
                author,
                publicationInfo,
                documentId);

            // Assert
            Assert.IsNotNull(citation);
            Assert.AreEqual(type, citation.Type);
            Assert.AreEqual(title, citation.Title);
            Assert.AreEqual(author, citation.Author);
            Assert.AreEqual(publicationInfo, citation.PublicationInfo);
            Assert.AreEqual(documentId, citation.FK_DocumentId);
            
            var storedCitation = _citationService?.GetCitationById(citation.Id);
            Assert.IsNotNull(storedCitation);
            Assert.AreEqual(citation.Id, storedCitation.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyTitle_ThrowsException()
        {
            // Act
            _citationService?.CreateCitation(
                CitationType.Book,
                "",
                "Author",
                "Publication Info",
                Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyAuthor_ThrowsException()
        {
            // Act
            _citationService?.CreateCitation(
                CitationType.Book,
                "Title",
                "",
                "Publication Info",
                Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyPublicationInfo_ThrowsException()
        {
            // Act
            _citationService?.CreateCitation(
                CitationType.Book,
                "Title",
                "Author",
                "",
                Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCitation_WithEmptyDocumentId_ThrowsException()
        {
            // Act
            _citationService?.CreateCitation(
                CitationType.Book,
                "Title",
                "Author",
                "Publication Info",
                Guid.Empty);
        }
        #endregion

        #region UpdateCitation
        [TestMethod]
        public void UpdateCitation_WhenCitationExists_UpdatesCitation()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var citation = new Citation
            {
                Type = CitationType.Book,
                Title = "Old Title",
                Author = "Old Author",
                PublicationInfo = "Old Publisher",
                FK_DocumentId = documentId
            };
            CitationRepository.AddCitation(citation);
            
            var newType = CitationType.JournalArticle;
            var newTitle = "New Title";
            var newAuthor = "New Author";
            var newPublicationInfo = "New Journal";
            var newPublicationDate = new DateTime(2023, 1, 1);
            var newAccessDate = "2023-05-15";
            var newDoi = "10.1234/abcd";

            // Act
            var updatedCitation = _citationService?.UpdateCitation(
                citation.Id,
                newType,
                newTitle,
                newAuthor,
                newPublicationInfo,
                newPublicationDate,
                newAccessDate,
                newDoi);

            // Assert
            Assert.IsNotNull(updatedCitation);
            Assert.AreEqual(citation.Id, updatedCitation.Id);
            Assert.AreEqual(newType, updatedCitation.Type);
            Assert.AreEqual(newTitle, updatedCitation.Title);
            Assert.AreEqual(newAuthor, updatedCitation.Author);
            Assert.AreEqual(newPublicationInfo, updatedCitation.PublicationInfo);
            Assert.AreEqual(newPublicationDate, updatedCitation.PublicationDate);
            Assert.AreEqual(newAccessDate, updatedCitation.AccessDate);
            Assert.AreEqual(newDoi, updatedCitation.DOI);
            Assert.IsTrue(updatedCitation.UpdatedAt > DateTime.MinValue);
            
            // Verify the repository was updated
            var storedCitation = CitationRepository.GetCitationById(citation.Id);
            Assert.IsNotNull(storedCitation);
            Assert.AreEqual(newTitle, storedCitation.Title);
        }

        [TestMethod]
        public void UpdateCitation_WhenCitationNotExists_ReturnsNull()
        {
            // Act
            var result = _citationService?.UpdateCitation(
                Guid.NewGuid(),
                CitationType.Book,
                "Title",
                "Author",
                "Publication Info");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateCitation_WithEmptyId_ThrowsException()
        {
            // Act
            _citationService?.UpdateCitation(
                Guid.Empty,
                CitationType.Book,
                "Title",
                "Author",
                "Publication Info");
        }
        #endregion

        #region DeleteCitation
        [TestMethod]
        public void DeleteCitation_WhenCitationExists_RemovesCitation()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var citation = new Citation
            {
                Type = CitationType.Book,
                Title = "Test Citation",
                Author = "Test Author",
                PublicationInfo = "Test Publisher",
                FK_DocumentId = documentId
            };
            CitationRepository.AddCitation(citation);

            // Act
            var result = _citationService?.DeleteCitation(citation.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(_citationService?.GetCitationById(citation.Id));
            Assert.AreEqual(0, _citationService?.GetAllCitations().Count);
        }

        [TestMethod]
        public void DeleteCitation_WhenCitationNotExists_ReturnsFalse()
        {
            // Act
            var result = _citationService?.DeleteCitation(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteCitation_WithEmptyId_ThrowsException()
        {
            // Act
            _citationService?.DeleteCitation(Guid.Empty);
        }
        #endregion

        #region GetFormattedCitationAPA
        [TestMethod]
        public void GetFormattedCitationAPA_ForBook_ReturnsCorrectFormat()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var author = "Smith, J.";
            var title = "The Test Book";
            var publisher = "Test Press";
            var publicationDate = new DateTime(2022, 1, 1);
            
            var citation = new Citation
            {
                Type = CitationType.Book,
                Title = title,
                Author = author,
                PublicationInfo = publisher,
                PublicationDate = publicationDate,
                FK_DocumentId = documentId
            };
            CitationRepository.AddCitation(citation);

            // Act
            var result = _citationService?.GetFormattedCitationAPA(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            var expected = $"{author}. (2022). {title}. {publisher}.";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFormattedCitationAPA_ForWebsite_ReturnsCorrectFormat()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var author = "Smith, J.";
            var title = "The Test Website";
            var url = "https://test.com";
            var publicationDate = new DateTime(2022, 1, 1);
            var accessDate = "May 15, 2023";
            
            var citation = new Citation
            {
                Type = CitationType.Website,
                Title = title,
                Author = author,
                PublicationInfo = url,
                PublicationDate = publicationDate,
                AccessDate = accessDate,
                FK_DocumentId = documentId
            };
            CitationRepository.AddCitation(citation);

            // Act
            var result = _citationService?.GetFormattedCitationAPA(citation.Id);

            // Assert
            Assert.IsNotNull(result);
            var expected = $"{author}. (2022). {title}. {url}. Diakses dari {accessDate}";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFormattedCitationAPA_WhenCitationNotExists_ReturnsNull()
        {
            // Act
            var result = _citationService?.GetFormattedCitationAPA(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFormattedCitationAPA_WithEmptyId_ThrowsException()
        {
            // Act
            _citationService?.GetFormattedCitationAPA(Guid.Empty);
        }
        #endregion
    }
}