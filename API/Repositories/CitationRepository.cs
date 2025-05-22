using API.Models;

namespace API.Repositories
{
    public class CitationRepository
    {
        // A static list to simulate a database collection for Citation objects.
        // This list will hold all Citation instances managed by this repository.
        private static readonly List<Citation> _citations = []; // Setara dengan 'new List<Citation>()'
        private static Citation? citationForService; // A placeholder for a single Citation instance.

        // Default constructor. Initializes the repository.
        public CitationRepository()
        {
            citationForService = new Citation();
        }

        // For first time initialization of the new Citation object.
        public CitationRepository(Guid id, CitationType type, string title, string author, string publicationInfo, Guid documentID)
        {
            citationForService = new Citation
            {
                Id = id,
                Type = type,
                Title = title,
                Author = author,
                PublicationInfo = publicationInfo,
                FK_DocumentId = documentID
            };
        }

        // For updating the Citation Object.
        public CitationRepository(Guid id, CitationType type, string title, string author, string publicationInfo, Guid documentID, DateTime updatedAt)
        {
            citationForService = new Citation
            {
                Id = id,
                Type = type,
                Title = title,
                Author = author,
                PublicationInfo = publicationInfo,
                FK_DocumentId = documentID,
                UpdatedAt = updatedAt
            };
        }

        public static List<Citation> Citations
        {
            get => _citations;
            set
            {
                if (value != null)
                {
                    _citations.Clear(); // Clear existing data
                    _citations.AddRange(value); // Add new data
                }
            }
        }

        public static void AddCitation(Citation citation)
        {
            if (citation == null)
            {
                throw new ArgumentNullException(nameof(citation), "Citation cannot be null.");
            }
            // Ensure the ID is set if it's empty (though the model defaults it)
            if (citation.Id == Guid.Empty)
            {
                citation.Id = Guid.NewGuid();
            }
            _citations.Add(citation);
        }

        public static Citation? GetCitationById(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
            }
            return _citations.FirstOrDefault(c => c.Id == citationId);
        }

        public static IEnumerable<Citation> GetCitationsByDocumentId(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("Document ID cannot be empty.", nameof(documentId));
            }
            // Order by Created_at for consistent retrieval, newest first
            return _citations.Where(c => c.FK_DocumentId == documentId)
                             .OrderByDescending(c => c.CreatedAt); // Ensure using Created_at property
        }

        public static bool UpdateCitation(Citation updatedCitation)
        {
            if (updatedCitation == null)
            {
                throw new ArgumentNullException(nameof(updatedCitation), "Updated citation cannot be null.");
            }
            if (updatedCitation.Id == Guid.Empty)
            {
                throw new ArgumentException("Updated citation ID cannot be empty.", nameof(updatedCitation));
            }

            var existingCitation = _citations.FirstOrDefault(c => c.Id == updatedCitation.Id);

            if (existingCitation != null)
            {
                // Update all modifiable properties
                existingCitation.Type = updatedCitation.Type;
                existingCitation.Title = updatedCitation.Title;
                existingCitation.Author = updatedCitation.Author;
                existingCitation.PublicationInfo = updatedCitation.PublicationInfo;
                existingCitation.PublicationDate = updatedCitation.PublicationDate;
                existingCitation.AccessDate = updatedCitation.AccessDate;
                existingCitation.DOI = updatedCitation.DOI;
                // existingCitation.FK_DocumentId = updatedCitation.FK_DocumentId; // Uncomment if FK_DocumentId can be updated
                existingCitation.UpdatedAt = DateTime.Now; // Set update timestamp

                return true;
            }
            return false; // Citation not found
        }

        public static bool DeleteCitation(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
            }
            var citationToRemove = _citations.FirstOrDefault(c => c.Id == citationId);
            if (citationToRemove != null)
            {
                _citations.Remove(citationToRemove);
                return true;
            }
            return false;
        }

        public static void ClearAllCitations()
        {
            _citations.Clear();
        }
    }
}