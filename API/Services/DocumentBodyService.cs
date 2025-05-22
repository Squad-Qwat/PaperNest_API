using API.Models;
using API.Repositories;
using System.Diagnostics;

namespace API.Services
{
    public class DocumentBodyService
    {
        public List<DocumentBody> GetAllDocumentBodies()
        {
            return DocumentBodyRepository.DocumentBodies;
        }
        public DocumentBody? GetDocumentBodyById(Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong");
            }

            return DocumentBodyRepository.GetDocumentBodyById(documentBodyId);
        }

        public IEnumerable<DocumentBody> GetDocumentBodiesByDocumentId(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            return DocumentBodyRepository.GetDocumentBodiesByDocumentId(documentId);
        }

        public DocumentBody CreateDocumentBody(
            Guid documentId,
            string content
            )
        {
            bool isCurrentVersion = true;
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content tidak boleh kosong");
            }
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            var currentVersion = DocumentBodyRepository.GetCurrentVersion(documentId);
            if (currentVersion != null)
            {
                currentVersion.IsCurrentVersion = false;
            }
            var documentBody = new DocumentBody
            {
                Content = content,
                FK_DocumentId = documentId,
                IsCurrentVersion = isCurrentVersion
            };
            DocumentBodyRepository.AddDocumentBody(documentBody);
            return documentBody;
        }

        public DocumentBody? GetCurrentVersion(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            return DocumentBodyRepository.GetCurrentVersion(documentId);
        }
    }
}
