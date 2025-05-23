using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

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
                // Note: The service method takes individual parameters, not the full object directly.
                // We map the request body properties to the service method parameters.
                if (newCitationRequest.Title == null || newCitationRequest.Author == null || newCitationRequest.PublicationInfo == null)
                {
                    return BadRequest(new { message = "Title, Author, and PublicationInfo are required." });
                }


                var createdCitation = _citationService.CreateCitation(
                    newCitationRequest.Type,
                    newCitationRequest.Title,
                    newCitationRequest.Author,
                    newCitationRequest.PublicationInfo,
                    newCitationRequest.FK_DocumentId,
                    newCitationRequest.PublicationDate,
                    newCitationRequest.AccessDate,
                    newCitationRequest.DOI
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

                if (updatedCitationRequest.Title == null || updatedCitationRequest.Author == null || updatedCitationRequest.PublicationInfo == null)
                {
                    return BadRequest(new { message = "Title, Author, and PublicationInfo are required." });
                }

                var updatedCitation = _citationService.UpdateCitation(
                    id, // Use the ID from the route
                    updatedCitationRequest.Type,
                    updatedCitationRequest.Title,
                    updatedCitationRequest.Author,
                    updatedCitationRequest.PublicationInfo,
                    updatedCitationRequest.PublicationDate,
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