using API.Models;
using API.Helpers.Enums;

namespace API.Repositories
{
    public class CitationRepository
    {
        
        
        private static readonly List<Citation> _citations = []; 
        private static Citation? citationForService; 

        
        public CitationRepository()
        {
            citationForService = new Citation();
        }

        
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
                    _citations.Clear(); 
                    _citations.AddRange(value); 
                }
            }
        }

        public static void AddCitation(Citation citation)
        {
            if (citation == null)
            {
                throw new ArgumentNullException(nameof(citation), "Citasi tidak boleh null");
            }
            
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
                throw new ArgumentException("ID citasi tidak boleh kosong", nameof(citationId));
            }
            return _citations.FirstOrDefault(c => c.Id == citationId);
        }

        public static IEnumerable<Citation> GetCitationsByDocumentId(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("ID dokumen tidak boleh kosong", nameof(documentId));
            }
            
            return _citations.Where(c => c.FK_DocumentId == documentId)
                            .OrderByDescending(c => c.CreatedAt); 
        }

        public static bool UpdateCitation(Citation updatedCitation)
        {
            if (updatedCitation == null)
            {
                throw new ArgumentNullException(nameof(updatedCitation), "Citasi yang diupdate tidak boleh null");
            }
            if (updatedCitation.Id == Guid.Empty)
            {
                throw new ArgumentException("ID citasi yang diupdate tidak boleh kosong", nameof(updatedCitation));
            }

            var existingCitation = _citations.FirstOrDefault(c => c.Id == updatedCitation.Id);

            if (existingCitation != null)
            {
                
                existingCitation.Type = updatedCitation.Type;
                existingCitation.Title = updatedCitation.Title;
                existingCitation.Author = updatedCitation.Author;
                existingCitation.PublicationInfo = updatedCitation.PublicationInfo;
                existingCitation.PublicationDate = updatedCitation.PublicationDate;
                existingCitation.AccessDate = updatedCitation.AccessDate;
                existingCitation.DOI = updatedCitation.DOI;
                
                existingCitation.UpdatedAt = DateTime.Now; 

                return true;
            }
            return false; 
        }

        public static bool DeleteCitation(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("ID citasi tidak boleh kosong", nameof(citationId));
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