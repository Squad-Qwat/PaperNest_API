using API.Models.DataBinding;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController, Route("api/document")]
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
            if (documentId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "Document ID cannot be empty"
                });
            }

            var versions = _documentBodyService.GetDocumentBodiesByDocumentId(documentId);
            
            if (versions == null || !versions.Any())
            {
                return NotFound(new
                {
                    message = "No versions found for this document"
                });
            }

            return Ok(versions);
        }

        [HttpPost("{documentId}/version")]
        public IActionResult CreateVersion(Guid documentId, [FromQuery] Guid userCreatorId, [FromBody] CreateDocumentBody createDocumentBody)
        {
            if(createDocumentBody == null)
            {
                return BadRequest(new
                {
                    message = "Invalid request body"
                });
            }

            if (string.IsNullOrWhiteSpace(createDocumentBody.Content) || string.IsNullOrWhiteSpace(createDocumentBody.Comment))
            {
                return BadRequest(new
                {
                    message = "Content and Comment cannot be empty"
                });
            }

            var version = _documentBodyService.CreateDocumentBody(documentId, userCreatorId, createDocumentBody.Comment, createDocumentBody.Content);
            
            if (version == null)
            {
                return BadRequest(new
                {
                    message = "Failed to create version"
                });
            }
            
            if (version.FK_DocumentId != documentId)
            {
                return BadRequest(new
                {
                    message = "Document ID mismatch"
                });
            }
            
            if (version.FK_UserCreatorId != userCreatorId)
            {
                return BadRequest(new
                {
                    message = "User Creator ID mismatch"
                });
            }

            return CreatedAtAction(nameof(GetVersions), new { documentId }, version);
        }

        [HttpGet("{documentId}/version/{documentBodyId}")]
        public IActionResult GetVersionInDocument(Guid documentId, Guid documentBodyId)
        {
            var version = _documentBodyService.GetDocumentBodyById(documentId, documentBodyId);
            
            if (version == null)
            {
                return NotFound();
            }

            if (version.FK_DocumentId != documentId)
            {
                return BadRequest(new
                {
                    message = "Document ID mismatch"
                });
            }

            if (version.Id != documentBodyId)
            {
                return BadRequest(new
                {
                    message = "Document Body ID mismatch"
                });
            }

            return Ok(version);
        }

        [HttpGet("{documentId}/version/current")]
        public IActionResult GetCurrentVersion(Guid documentId)
        {
            var version = _documentBodyService.GetCurrentVersion(documentId);
            if (version == null)
            {
                return NotFound();
            }

            if (version.FK_DocumentId != documentId)
            {
                return BadRequest(new
                {
                    message = "Document ID mismatch"
                });
            }

            if (!version.IsCurrentVersion)
            {
                return BadRequest(new
                {
                    message = "This version is not the current version"
                });
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
            if (!isRemoved)
            {
                return BadRequest(new
                {
                    message = "Gagal menghapus version"
                });
            }
            if (version.FK_DocumentId != documentId)
            {
                return BadRequest(new
                {
                    message = "Document ID mismatch"
                });
            }

            if (version.Id != documentBodyId)
            {
                return BadRequest(new
                {
                    message = "Document Body ID mismatch"
                });
            }

            return Ok(new
            {
                message = "Berhasil menghapus version"
            });
        }
    }
}