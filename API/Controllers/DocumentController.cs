using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;

namespace PaperNest_API.Controllers
{
    [ApiController, Route("api/documents")]
    public class DocumentController(DocumentService documentService) : Controller
    {
        public readonly DocumentService _documentService = documentService;

        /*
         * Setara dengan:
         * public DocumentController(DocumentService documentService)
         * {
         *    _documentService = documentService;
         * }
         */

        [HttpGet]
        public IActionResult GetAllDocuments()
        {
            var documents = _documentService.GetAll();

            if (documents == null || !documents.Any())
            {
                return NotFound(new
                {
                    message = "No documents found"
                });
            }

            return Ok(new
            {
                message = "Success get all documents",
                data = documents
            });
        }

        [HttpGet("{documentid}")]
        public IActionResult GetDocumentById(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "Invalid document ID"
                });
            }

            var document = _documentService.GetById(documentId);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Doccument not found"
                });
            }

            return Ok(new
            {
                message = "Success get all documents",
                data = document
            });
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetDocumentsByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "Invalid user ID"
                });
            }

            var documents = _documentService.GetByUserId(userId);

            if (documents == null || !documents.Any())
            {
                return NotFound(new
                {
                    message = "No documents found for this user"
                });
            }

            return Ok(new
            {
                message = "Success get document owned by user",
                data = documents
            });
        }

        [HttpGet("workspace/{workspaceId}")]
        public IActionResult GetDocumentsByWorkspaceId(Guid workspaceId)
        {
            if (workspaceId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "Invalid workspace ID"
                });
            }

            var documents = _documentService.GetByWorkspaceId(workspaceId);

            if (documents == null || !documents.Any())
            {
                return NotFound(new
                {
                    message = "No documents found in this workspace"
                });
            }

            return Ok(new
            {
                message = "Success get all documents in workspace",
                data = documents
            });
        }

        [HttpPost]
        public IActionResult CreateDocument([FromBody] Document newDocument)
        {
            if (newDocument == null)
            {
                return BadRequest(new
                {
                    message = "Document data is required"
                });
            }

            _documentService.Create(newDocument);

            return CreatedAtAction(nameof(GetDocumentById), new { id = newDocument.Id }, new
            {
                message = "Success create new document",
                data = newDocument
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDocument(Guid id, [FromBody] Document updatedDocument)
        {
            if (updatedDocument == null)
            {
                return BadRequest(new
                {
                    message = "Updated document data is required"
                });
            }

            if (id != updatedDocument.Id)
            {
                return Unauthorized(new
                {
                    message = "Document ID in the URL does not match the ID in the document data"
                });
            }

            var existingDocument = _documentService.GetById(id);

            if (existingDocument == null)
            {
                return NotFound(new
                {
                    message = "Document not found"
                });
            }

            _documentService.Update(id, updatedDocument);

            return Ok(new
            {
                message = "Success to update document",
                data = _documentService.GetById(id)
            });
        }

        [HttpDelete("{documentid}")]
        public IActionResult DeleteDocument(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "Invalid document ID"
                });
            }

            var existingDocument = _documentService.GetById(documentId);

            if (existingDocument == null)
            {
                return NotFound(new
                {
                    message = "Document not found"
                });
            }

            _documentService.Delete(documentId);

            return Ok(new
            {
                message = "Document has been deleted"
            });
        }
    }
}