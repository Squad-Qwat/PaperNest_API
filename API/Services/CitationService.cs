using API.Repositories;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Helpers.Enums;
using API.StateMachineAndUtils;

namespace API.Services
{
    public class CitationService
    {
        public List<Citation> GetAllCitations()
        {
            try
            {
                if (CitationRepository.Citations == null)
                {
                    throw new InvalidOperationException("Citation repository is not initialized.");
                }
                return CitationRepository.Citations;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error retrieving citations: {ex.Message}");
                return []; // Setara dengan 'new List<Citation>()' untuk mengembalikan daftar kosong
            }
        }

        public Citation? GetCitationById(Guid citationId)
        {
            try
            {
                if (citationId == Guid.Empty)
                {
                    throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
                }
                return CitationRepository.GetCitationById(citationId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving citation: {ex.Message}");
                return null;
            }
        }

        public IEnumerable<Citation> GetCitationsByDocumentId(Guid documentId)
        {
            try
            {
                if (documentId == Guid.Empty)
                {
                    throw new ArgumentException("Document ID cannot be empty.", nameof(documentId));
                }
                return CitationRepository.GetCitationsByDocumentId(documentId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving citations by document ID: {ex.Message}");
                return []; // Setara dengan 'Enumerable.Empty<Citation>' untuk mengembalikan daftar kosong
            }
        }
        public Citation? CreateCitation(
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
            try
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
                else if (!CitationRepository.GetCitationsByDocumentId((Guid)documentId).Any())
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
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error creating citation: {ex.Message}");
                return null; // Return null if there was an error
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating citation: {ex.Message}");
                return null; // Return null if there was an error
            }
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
            try
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

                var existingCitation = CitationRepository.GetCitationById(citationId) ??
                    throw new ArgumentException("Citation with the specified ID does not exist.", nameof(citationId));
                /*
                 * Setara dengan:
                 * if (existingCitation == null)
                 * {
                 *    throw new ArgumentException("Citation with the specified ID does not exist.", nameof(citationId));
                 * }
                */
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

                // Since the if-else structure is inverse, the bool return type is not needed here.
                var updatedCitation = CitationRepository.UpdateCitation(existingCitation);

                return updatedCitation;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating citation: {ex.Message}");
                return null; // Return null if there was an error
            }
        }

        public bool DeleteCitation(Guid citationId)
        {
            try
            {
                if (citationId == Guid.Empty)
                {
                    throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
                }
                return CitationRepository.DeleteCitation(citationId);
            }
            catch (ArgumentException ex) 
            {
                Console.WriteLine($"Error deleting citation: {ex.Message}");
                return false; // Return false if there was an error
            }
        }

        public string? GetFormattedCitationAPA(Guid citationId)
        {
            try
            {
                if (citationId == Guid.Empty)
                {
                    throw new ArgumentException("Citation ID cannot be empty.", nameof(citationId));
                }

                var citation = CitationRepository.GetCitationById(citationId) ?? 
                    throw new ArgumentException("Citation with the specified ID does not exist.", nameof(citationId));
                /*
                 * Setara dengan:
                    'if (citation == null)
                    {
                        throw new ArgumentException("Citation with the specified ID does not exist.", nameof(citationId));
                    }'
                */

                return CitationFormatter.GenerateAPAStyle(citation);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error formatting citation: {ex.Message}");
                return null; // Return null if there was an error
            }
        }
    }
}