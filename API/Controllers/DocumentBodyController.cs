using API.Models.DataBinding;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController,Route("api/document")]
    public class DocumentBodyController : Controller
    {
        private readonly DocumentBodyService _documentBodyService;
        public DocumentBodyController(DocumentBodyService documentBodyService)
        {
            _documentBodyService = documentBodyService;
        }

        [HttpGet("{documentId}/versions")]
        public IActionResult GetVersions(Guid documentId)
        {
            try
            {
                var versions = _documentBodyService.GetDocumentBodiesByDocumentId(documentId);
                if (versions == null || !versions.Any())
                {
                    return NotFound(new
                    {
                        message = "Document tidak ditemukan"
                    });
                }
                return Ok(versions);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil versions" });
            }
        }

        [HttpPost("{documentId}/version")]
        public IActionResult CreateVersion(Guid documentId,[FromQuery] Guid userCreatorId, [FromBody] CreateDocumentBody createDocumentBody)
        {
            try
            {
                var version = _documentBodyService.CreateDocumentBody(documentId, userCreatorId, createDocumentBody.Comment, createDocumentBody.Content);
                if (version == null)
                {
                    return BadRequest(new
                    {
                        message = "Gagal membuat version"
                    });
                }
                return CreatedAtAction(nameof(GetVersions), new { documentId }, version);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat membuat version" });
            }
        }

        [HttpGet("{documentId}/version/{documentBodyId}")]
        public IActionResult GetVersionInDocument(Guid documentId,Guid documentBodyId)
        {
            try
            {
                var version = _documentBodyService.GetDocumentBodyById(documentId, documentBodyId);
                if (version == null)
                {
                    return NotFound(new
                    {
                        message = "Document tidak ditemukan"
                    });
                }
                return Ok(version);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil version" });
            }
        }

        [HttpGet("{documentId}/version/current")]
        public IActionResult GetCurrentVersion(Guid documentId)
        {
            try
            {
                var version = _documentBodyService.GetCurrentVersion(documentId);
                if (version == null)
                {
                    return NotFound(new
                    {
                        message = "Document tidak ditemukan"
                    });
                }
                return Ok(version);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil current version" });
            }
        }

        [HttpDelete("{documentId}/version/{documentBodyId}")]
        public IActionResult DeleteVersion(Guid documentId, Guid documentBodyId)
        {
            try
            {
                var version = _documentBodyService.GetDocumentBodyById(documentId, documentBodyId);
                if (version == null)
                {
                    return NotFound(new
                    {
                        message = "Document tidak ditemukan"
                    });
                }
                var isRemoved = _documentBodyService.RemoveDocumentBody(documentId, documentBodyId);
                if (isRemoved)
                {
                    return Ok(new
                    {
                        message = "Berhasil menghapus version"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Gagal menghapus version"
                    });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus version" });
            }
        }

    }
}
