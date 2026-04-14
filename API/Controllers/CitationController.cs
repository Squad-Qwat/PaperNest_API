using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/citations")]
    public class CitationController : ControllerBase 
    {
        private readonly CitationService _citationService;

        
        public CitationController(CitationService citationService)
        {
            _citationService = citationService;
        }

        [HttpGet]
        public IActionResult GetAllCitations()
        {
            try
            {
                var citations = _citationService.GetAllCitations();
                return Ok(new
                {
                    message = "Berhasil mengambil semua citasi",
                    data = citations
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil semua citasi" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCitationById(Guid id)
        {
            try
            {
                var citation = _citationService.GetCitationById(id);
                if (citation == null)
                {
                    return NotFound(new { message = "Citasi tidak ditemukan" });
                }
                return Ok(new
                {
                    message = "Berhasil mengambil citasi berdasarkan ID",
                    data = citation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil citasi" });
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
                    message = "Berhasil mengambil citasi berdasarkan ID dokumen",
                    data = citations
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil citasi dokumen" });
            }
        }

        [HttpPost]
        public IActionResult CreateCitation([FromBody] Citation newCitationRequest)
        {
            try
            {
                
                
                if (newCitationRequest.Title == null || newCitationRequest.Author == null || newCitationRequest.PublicationInfo == null)
                {
                    return BadRequest(new { message = "Title, Author, dan PublicationInfo wajib diisi" });
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
                    message = "Berhasil membuat citasi baru",
                    data = createdCitation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat membuat citasi" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCitation(Guid id, [FromBody] Citation updatedCitationRequest)
        {
            try
            {
                
                if (id != updatedCitationRequest.Id && updatedCitationRequest.Id != Guid.Empty)
                {
                    return BadRequest(new { message = "ID di route dan request body tidak cocok" });
                }

                if (updatedCitationRequest.Title == null || updatedCitationRequest.Author == null || updatedCitationRequest.PublicationInfo == null)
                {
                    return BadRequest(new { message = "Title, Author, dan PublicationInfo wajib diisi" });
                }

                var updatedCitation = _citationService.UpdateCitation(
                    id, 
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
                    return NotFound(new { message = "Citasi tidak ditemukan atau gagal diupdate" });
                }

                return Ok(new
                {
                    message = "Berhasil mengupdate citasi",
                    data = updatedCitation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengupdate citasi" });
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
                    return NotFound(new { message = "Citasi tidak ditemukan" });
                }
                return Ok(new
                {
                    message = "Citasi berhasil dihapus"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus citasi" });
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
                    return NotFound(new { message = "Citasi tidak ditemukan atau tidak dapat diformat" });
                }
                return Ok(new
                {
                    message = "Berhasil mengambil citasi dalam format APA",
                    data = formattedCitation
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat memformat citasi" });
            }
        }
    }
}