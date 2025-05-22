using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;

namespace PaperNest_API.Controllers
{
    [ApiController, Route("api/documents")]
    public class DocumentController : Controller
    {
        [HttpGet]
        public IActionResult GetAllDocuments()
        {
            var documents = DocumentService.GetAll();

            return Ok(new
            {
                message = "Success get all documents",
                data = documents
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetDocumentById(Guid id)
        {
            var document = DocumentService.GetById(id);

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
            var documents = DocumentService.GetByUserId(userId);

            return Ok(new
            {
                message = "Success get document owned by user" ,
                data = documents
            });
        }

        [HttpGet("workspace/{workspaceId}")]
        public IActionResult GetDocumentsByWorkspaceId(Guid workspaceId)
        {
            var documents = DocumentService.GetByWorkspaceId(workspaceId);

            return Ok(new
            {
                message = "Success get all documents in workspace",
                data = documents
            });
        }

        [HttpPost]
        public IActionResult CreateDocument([FromBody] Document document)
        {
            DocumentService.Create(document);

            return CreatedAtAction(nameof(GetDocumentById), new { id = document.Id }, new
            {
                message = "Success create new document",
                data = document
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDocument(Guid id, [FromBody] Document document)
        {
            var existingDocument = DocumentService.GetById(id);

            if (existingDocument == null)
            {
                return NotFound(new
                {
                    message = "Document not found"
                });
            }

            DocumentService.Update(id, document);

            return Ok(new
            {
                message = "Success to update document",
                data = DocumentService.GetById(id)
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDocument(Guid id)
        {
            var existingDocument = DocumentService.GetById(id);

            if (existingDocument == null)
            {
                return NotFound(new
                {
                    message = "Document not found"
                });
            }

            DocumentService.Delete(id);

            return Ok(new
            {
                message = "Success delete document"
            });
        }
    }
}