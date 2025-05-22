using API.Models;

namespace API.Repositories
{
    public class CitationRepository
    {
        // A static list to simulate a database collection for Citation objects.
        // This list will hold all Citation instances managed by this repository.
        private static readonly List<Citation> _citations = []; // Setara dengan `new List<Citation>()` untuk menyimpan semua Citation instances.
        private static Citation? citationForService; // A placeholder for a single Citation instance.

        // Default constructor. Initializes the repository.
        public CitationRepository()
        {
            citationForService = new Citation();
        }

        // For first time initialization of the new Citation object.
        // Updated constructor to include new properties for initialization
        public CitationRepository(Guid id, CitationType type, string title, string author, string pages, string volume, string issue, string url, string accessURL, string accessLocation, string publicationInfo, string publisher, DateTime? publicationDate, string publisherLocation, string accessDate, string doi, Guid documentID)
        {
            citationForService = new Citation
            {
                Id = id,
                Type = type,
                Title = title,
                Author = author,
                Pages = pages,
                Volume = volume,
                Issue = issue,
                URL = url,
                AccessURL = accessURL,
                AccessLocation = accessLocation,
                PublicationInfo = publicationInfo,
                Publisher = publisher,
                PublicationDate = publicationDate,
                PublisherLocation = publisherLocation,
                AccessDate = accessDate,
                DOI = doi,
                FK_DocumentId = documentID
            };
        }

        // For updating the Citation Object.
        // Updated constructor to include new properties for initialization
        public CitationRepository(Guid id, CitationType type, string title, string author, string pages, string volume, string issue, string url, string accessURL, string accessLocation, string publicationInfo, string publisher, DateTime? publicationDate, string publisherLocation, string accessDate, string doi, Guid documentID, DateTime updatedAt)
        {
            citationForService = new Citation
            {
                Id = id,
                Type = type,
                Title = title,
                Author = author,
                Pages = pages,
                Volume = volume,
                Issue = issue,
                URL = url,
                AccessURL = accessURL,
                AccessLocation = accessLocation,
                PublicationInfo = publicationInfo,
                Publisher = publisher,
                PublicationDate = publicationDate,
                PublisherLocation = publisherLocation,
                AccessDate = accessDate,
                DOI = doi,
                FK_DocumentId = documentID,
                UpdatedAt = updatedAt
            };
        }

        public static List<Citation> Citations
        {
            get => _citations;
            set
            {
                try 
                { 
                    if (value == null)
                    {
                        Console.WriteLine("Setting new citations list.");
                        return;
                    }

                    if (value.Count == 0)
                    {
                        Console.WriteLine("Setting empty citations list.");
                    }
                    else
                    {
                        Console.WriteLine($"Setting citations list with {value.Count} items.");
                    }

                    if (value.Any(c => c == null))
                    {
                        throw new ArgumentException("Citations list cannot contain null items.");
                    }

                    if (value.Any(c => c.Id == Guid.Empty))
                    {
                        throw new ArgumentException("Citations list cannot contain items with empty IDs.");
                    }
                    _citations.Clear(); // Clear existing data
                    _citations.AddRange(value); // Add new data
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error setting citations: {ex.Message}");
                }
            }
        }

        public static void AddCitation(Citation citation)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error adding citation: {ex.Message}");
            } 
        }

        public static Citation? GetCitationById(Guid citationId)
        {
            try
            {
                if (citationId == Guid.Empty)
                {
                    throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
                }

                return _citations.FirstOrDefault(c => c.Id == citationId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving citation: {ex.Message}");
                return null;
            } 
        }

        public static IEnumerable<Citation> GetCitationsByDocumentId(Guid documentId)
        {
            try
            {
                if (documentId == Guid.Empty)
                {
                    throw new ArgumentException("Document ID cannot be empty.", nameof(documentId));
                }
                // Order by CreatedAt for consistent retrieval, newest first
                return _citations.Where(c => c.FK_DocumentId == documentId)
                                 .OrderByDescending(c => c.CreatedAt); // Ensure using CreatedAt property
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving citations: {ex.Message}");
                return []; // Setara dengan 'Enumerable.Empty<Citation>()'
            }
        }

        public static bool UpdateCitation(Citation updatedCitation)
        {
            try
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

                if (existingCitation == null)
                {
                    throw new Exception($"Citation with ID {updatedCitation.Id} not found.");
                }
                
                // Update all modifiable properties
                existingCitation.Type = updatedCitation.Type;
                existingCitation.Title = updatedCitation.Title;
                existingCitation.Author = updatedCitation.Author;
                existingCitation.Pages = updatedCitation.Pages; // New
                existingCitation.Volume = updatedCitation.Volume; // New
                existingCitation.Issue = updatedCitation.Issue; // New
                existingCitation.URL = updatedCitation.URL; // New
                existingCitation.AccessURL = updatedCitation.AccessURL; // New
                existingCitation.AccessLocation = updatedCitation.AccessLocation; // New
                existingCitation.PublicationInfo = updatedCitation.PublicationInfo;
                existingCitation.Publisher = updatedCitation.Publisher; // New
                existingCitation.PublicationDate = updatedCitation.PublicationDate;
                existingCitation.PublisherLocation = updatedCitation.PublisherLocation; // New
                existingCitation.AccessDate = updatedCitation.AccessDate;
                existingCitation.DOI = updatedCitation.DOI;
                // existingCitation.FK_DocumentId = updatedCitation.FK_DocumentId; // Uncomment if FK_DocumentId can be updated
                existingCitation.UpdatedAt = DateTime.Now; // Set update timestamp

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error updating citation: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteCitation(Guid citationId)
        {
            try
            {
                if (citationId == Guid.Empty)
                {
                    throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
                }
                var citationToRemove = _citations.FirstOrDefault(c => c.Id == citationId) ?? throw new Exception($"Citation with ID {citationId} not found.");
                /*
                 * Setara dengan 'if (citationToRemove == null)
                 * { 
                 *     throw new Exception($"Citation with ID {citationId} not found.");
                 * }'
                 */
                _citations.Remove(citationToRemove);
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error deleting citation: {ex.Message}");
                return false;
            } 
        }

        public static void ClearAllCitations()
        {
            _citations.Clear();
        }
    }
}