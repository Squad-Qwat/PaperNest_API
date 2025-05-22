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
        public IActionResult CreateVersion(Guid documentId, [FromBody] string content)
        {
            var version = _documentBodyService.CreateDocumentBody(documentId, content);
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

    }
}
