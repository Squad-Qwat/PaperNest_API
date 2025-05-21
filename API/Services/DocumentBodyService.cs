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

        public DocumentBody CreateVersion(Guid documentId, string content)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content tidak boleh kosong");
            }
            var documentBody = new DocumentBody
            {
                Content = content,
                FK_DocumentId = documentId,
                IsCurrentVersion = true
            };
            DocumentBodyRepository.AddDocumentBody(documentBody);
            return documentBody;
        }

        public IEnumerable<DocumentBody> GetVersions(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            return DocumentBodyRepository.GetDocumentBodiesByDocumentId(documentId);
        }

        public void AddDocumentBody(
            string content,
            Guid documentId,
            bool isCurrentVersion = false
            )
        {
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
