using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using API.Models; // For using Citation model and CitationType enum
using API.Services; // For using CitationService class

namespace API.Controllers
{
    [ApiController]
    [Route("api/citations")]
    public class CitationController : ControllerBase // Use ControllerBase for API controllers without view support
    {
        private readonly CitationService _citationService;

        // Constructor for dependency injection of CitationService
        public CitationController(CitationService citationService)
        {
            _citationService = citationService;
        }

        [HttpGet]
        public IActionResult GetAllCitations()
        {
            var citations = _citationService.GetAllCitations();
            return Ok(new
            {
                message = "Success get all citations",
                data = citations
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetCitationById(Guid id)
        {
            try
            {
                var citation = _citationService.GetCitationById(id);
                if (citation == null)
                {
                    return NotFound(new { message = "Citation not found" });
                }
                return Ok(new
                {
                    message = "Success get citation by ID",
                    data = citation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("document/{documentId}")]
        public IActionResult GetCitationsByDocumentId(Guid documentId)
        {
            try
            {
                var citations = _citationService.GetCitationsByDocumentId(documentId);
                return Ok(new
                {
                    message = "Success get citations by document ID",
                    data = citations
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateCitation([FromBody] Citation newCitationRequest)
        {
            try
            {
                // Basic validation for required fields
                if (string.IsNullOrWhiteSpace(newCitationRequest.Title) ||
                    string.IsNullOrWhiteSpace(newCitationRequest.Author))
                {
                    return BadRequest(new { message = "Title and Author are required." });
                }

                var createdCitation = _citationService.CreateCitation(
                    newCitationRequest.Type,
                    newCitationRequest.Title,
                    newCitationRequest.Author,
                    newCitationRequest.Pages,          // New
                    newCitationRequest.Volume,         // New
                    newCitationRequest.Issue,          // New
                    newCitationRequest.URL,            // New
                    newCitationRequest.AccessURL,      // New
                    newCitationRequest.AccessLocation, // New
                    newCitationRequest.PublicationInfo,
                    newCitationRequest.Publisher,      // New
                    newCitationRequest.PublicationDate,
                    newCitationRequest.PublisherLocation, // New
                    newCitationRequest.AccessDate,
                    newCitationRequest.DOI,
                    newCitationRequest.FK_DocumentId // documentId is now optional in service, but still passed from request
                );

                return CreatedAtAction(nameof(GetCitationById), new { id = createdCitation.Id }, new
                {
                    message = "Success create new citation",
                    data = createdCitation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the citation.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCitation(Guid id, [FromBody] Citation updatedCitationRequest)
        {
            try
            {
                // Ensure the ID in the route matches the ID in the request body (if provided)
                if (id != updatedCitationRequest.Id && updatedCitationRequest.Id != Guid.Empty)
                {
                    return BadRequest(new { message = "Mismatched ID in route and request body." });
                }

                // Basic validation for required fields
                if (string.IsNullOrWhiteSpace(updatedCitationRequest.Title) ||
                    string.IsNullOrWhiteSpace(updatedCitationRequest.Author))
                {
                    return BadRequest(new { message = "Title and Author are required." });
                }

                var updatedCitation = _citationService.UpdateCitation(
                    id, // Use the ID from the route
                    updatedCitationRequest.Type,
                    updatedCitationRequest.Title,
                    updatedCitationRequest.Author,
                    updatedCitationRequest.Pages,          // New
                    updatedCitationRequest.Volume,         // New
                    updatedCitationRequest.Issue,          // New
                    updatedCitationRequest.URL,            // New
                    updatedCitationRequest.AccessURL,      // New
                    updatedCitationRequest.AccessLocation, // New
                    updatedCitationRequest.PublicationInfo,
                    updatedCitationRequest.Publisher,      // New
                    updatedCitationRequest.PublicationDate,
                    updatedCitationRequest.PublisherLocation, // New
                    updatedCitationRequest.AccessDate,
                    updatedCitationRequest.DOI
                );

                if (updatedCitation == null)
                {
                    return NotFound(new { message = "Citation not found or update failed" });
                }

                return Ok(new
                {
                    message = "Success to update citation",
                    data = updatedCitation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the citation.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCitation(Guid id)
        {
            try
            {
                var isDeleted = _citationService.DeleteCitation(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = "Citation not found" });
                }
                return Ok(new
                {
                    message = "Citation has been deleted"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the citation.", error = ex.Message });
            }
        }

        [HttpGet("{id}/apa")]
        public IActionResult GetCitationApaStyle(Guid id)
        {
            try
            {
                var formattedCitation = _citationService.GetFormattedCitationAPA(id);
                if (formattedCitation == null)
                {
                    return NotFound(new { message = "Citation not found or could not be formatted." });
                }
                return Ok(new
                {
                    message = "Success get APA style citation",
                    data = formattedCitation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while formatting the citation.", error = ex.Message });
            }
        }
    }
}