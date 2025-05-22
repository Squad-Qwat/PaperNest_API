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
            var versions = _documentBodyService.GetDocumentBodiesByDocumentId(documentId);
            return Ok(versions); 
        }

        [HttpPost("{documentId}/version")]
        public IActionResult CreateVersion(Guid documentId,[FromQuery] Guid userCreatorId, [FromBody] string content)
        {
            var version = _documentBodyService.CreateDocumentBody(documentId, userCreatorId, content);
            return CreatedAtAction(nameof(GetVersions), new { documentId }, version);
        }

        [HttpGet("{documentId}/version/{documentBodyId}")]
        public IActionResult GetVersionInDocument(Guid documentId,Guid documentBodyId)
        {
            var version = _documentBodyService.GetDocumentBodyById(documentId, documentBodyId);
            if (version == null)
            {
                return NotFound();
            }
            return Ok(version);
        }

        [HttpDelete("{documentId}/version/{documentBodyId}")]
        public IActionResult DeleteVersion(Guid documentId, Guid documentBodyId)
        {
            var version = _documentBodyService.GetDocumentBodyById(documentId, documentBodyId);
            if (version == null)
            {
                return NotFound();
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

    }
}
