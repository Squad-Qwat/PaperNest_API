using API.Repositories;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.StateMachines;

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
                throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
            }
            return CitationRepository.GetCitationById(citationId);
        }

        public IEnumerable<Citation> GetCitationsByDocumentId(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("Document ID cannot be empty.", nameof(documentId));
            }
            return CitationRepository.GetCitationsByDocumentId(documentId);
        }

        public Citation CreateCitation(
            CitationType type,
            string title,
            string author,
            string? pages = null,
            string? volume = null,
            string? issue = null,
            string? url = null,
            string? accessURL = null,
            string? accessLocation = null,
            string? publicationInfo = null,
            string? publisher = null,
            DateTime? publicationDate = null,
            string? publisherLocation = null,
            string? accessDate = null,
            string? doi = null,
            Guid? documentId = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Citation title cannot be null or empty.", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Citation author cannot be null or empty.", nameof(author));
            }
            // PublicationInfo is now optional based on the Citation class, so no validation here.
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("Document ID cannot be empty.", nameof(documentId));
            }
            else if (documentId == null) 
            {
                throw new ArgumentNullException(nameof(documentId), "Document ID cannot be null.");
            }
            else if (CitationRepository.GetCitationsByDocumentId((Guid)documentId).Count() == 0)
            {
                throw new ArgumentException("Document ID does not exist.", nameof(documentId));
            }

                var newCitation = new Citation
                {
                    Type = type,
                    Title = title,
                    Author = author,
                    Pages = pages, // New
                    Volume = volume, // New
                    Issue = issue, // New
                    URL = url, // New
                    AccessURL = accessURL, // New
                    AccessLocation = accessLocation, // New
                    PublicationInfo = publicationInfo,
                    Publisher = publisher, // New
                    PublicationDate = publicationDate,
                    PublisherLocation = publisherLocation, // New
                    AccessDate = accessDate,
                    DOI = doi,
                    FK_DocumentId = (Guid)documentId,
                    // Id and CreatedAt are set by the Citation model's default constructor
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
            string? pages = null,
            string? volume = null,
            string? issue = null,
            string? url = null,
            string? accessURL = null,
            string? accessLocation = null,
            string? publicationInfo = null,
            string? publisher = null,
            DateTime? publicationDate = null,
            string? publisherLocation = null,
            string? accessDate = null,
            string? doi = null)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Citation title cannot be null or empty.", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Citation author cannot be null or empty.", nameof(author));
            }
            // PublicationInfo is now optional, so no validation here.

            var existingCitation = CitationRepository.GetCitationById(citationId);

            if (existingCitation != null)
            {
                // Update all modifiable properties
                existingCitation.Type = type;
                existingCitation.Title = title;
                existingCitation.Author = author;
                existingCitation.Pages = pages; // New
                existingCitation.Volume = volume; // New
                existingCitation.Issue = issue; // New
                existingCitation.URL = url; // New
                existingCitation.AccessURL = accessURL; // New
                existingCitation.AccessLocation = accessLocation; // New
                existingCitation.PublicationInfo = publicationInfo;
                existingCitation.Publisher = publisher; // New
                existingCitation.PublicationDate = publicationDate;
                existingCitation.PublisherLocation = publisherLocation; // New
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
                throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
            }
            return CitationRepository.DeleteCitation(citationId);
        }

        public string? GetFormattedCitationAPA(Guid citationId)
        {
            if (citationId == Guid.Empty)
            {
                throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
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