using API.Helpers;
using API.Helpers.Enums;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class CitationService
    {
        public List<Citation> GetAllCitations()
        {
            return CitationRepository.Citations;
        }

        public Citation? GetCitationById(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("ID citasi tidak boleh kosong", nameof(citationId));
            }
            return CitationRepository.GetCitationById(citationId);
        }

        public IEnumerable<Citation> GetCitationsByDocumentId(Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("ID DocumentBody tidak boleh kosong", nameof(documentBodyId));
            }
            return CitationRepository.GetCitationsByDocumentId(documentBodyId);
        }

        public Citation CreateCitation(CitationType type,
            string title,
            string author,
            string publicationInfo,
            Guid documentId,
            DateTime? publicationDate = null,
            string? accessDate = null,
            string? doi = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Judul citasi tidak boleh kosong", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Penulis citasi tidak boleh kosong", nameof(author));
            }
            if (string.IsNullOrWhiteSpace(publicationInfo))
            {
                throw new ArgumentException("Informasi publikasi citasi tidak boleh kosong", nameof(publicationInfo));
            }
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("ID dokumen tidak boleh kosong", nameof(documentId));
            }

            var newCitation = new Citation
            {
                Type = type,
                Title = title,
                Author = author,
                PublicationInfo = publicationInfo,
                FK_DocumentId = documentId,
                PublicationDate = publicationDate,
                AccessDate = accessDate,
                DOI = doi,
                // Id and Created_at are set by the Citation model's default constructor
            };

            // Add the new citation using the repository
            CitationRepository.AddCitation(newCitation);

            return newCitation;
        }

        public Citation? UpdateCitation(
            Guid citationId,
            CitationType type,
            string title,
            string author,
            string publicationInfo,
            DateTime? publicationDate = null,
            string? accessDate = null,
            string? doi = null)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("ID citasi tidak boleh kosong", nameof(citationId));
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Judul citasi tidak boleh kosong", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Penulis citasi tidak boleh kosong", nameof(author));
            }
            if (string.IsNullOrWhiteSpace(publicationInfo))
            {
                throw new ArgumentException("Informasi publikasi citasi tidak boleh kosong", nameof(publicationInfo));
            }

            var existingCitation = CitationRepository.GetCitationById(citationId);

            if (existingCitation != null)
            {
                // Update all modifiable properties
                existingCitation.Type = type;
                existingCitation.Title = title;
                existingCitation.Author = author;
                existingCitation.PublicationInfo = publicationInfo;
                existingCitation.PublicationDate = publicationDate;
                existingCitation.AccessDate = accessDate;
                existingCitation.DOI = doi;
                existingCitation.UpdatedAt = DateTime.Now; // Set update timestamp

                // Call the repository's update method
                if (CitationRepository.UpdateCitation(existingCitation))
                {
                    return existingCitation;
                }
            }
            return null; // Citation not found or update failed in repository
        }

        public bool DeleteCitation(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("ID citasi tidak boleh kosong", nameof(citationId));
            }
            return CitationRepository.DeleteCitation(citationId);
        }

        public string? GetFormattedCitationAPA(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("ID citasi tidak boleh kosong", nameof(citationId));
            }

            var citation = CitationRepository.GetCitationById(citationId);

            if (citation != null)
            {
                return CitationFormatter.GenerateAPAStyle(citation);
            }

            return null; // Citation not found
        }
    }
}