using API.Models;
using API.Repositories;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace API.Services
{
    public class DocumentBodyService
    {
        public List<DocumentBody> GetAllDocumentBodies()
        {
            return DocumentBodyRepository.DocumentBodies;
        }
        public DocumentBody? GetDocumentBodyById(Guid documentId, Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong");
            }

            return DocumentBodyRepository.GetDocumentBodyById(documentId ,documentBodyId);
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
            Guid userCreatorId,
            string comment,
            string content
            )
        {
            bool isCurrentVersion = true;
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content tidak boleh kosong");
            }
            if (documentId == Guid.Empty || userCreatorId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId atau UserCreatorId tidak boleh kosong");
            }
            var currentVersion = DocumentBodyRepository.GetCurrentVersion(documentId);
            if (currentVersion != null)
            {
                currentVersion.IsCurrentVersion = false;
            }
            var documentBody = new DocumentBody
            {
                Content = content,
                Comment = comment,
                FK_DocumentId = documentId,
                FK_UserCreaotorId = userCreatorId,
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

        public bool RemoveDocumentBody(Guid documentId, Guid documentBodyId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong");
            }
            var documentBody = DocumentBodyRepository.GetDocumentBodyById(documentId, documentBodyId);
            if (documentBody != null)
            {
                return DocumentBodyRepository.DeleteDocumentBody(documentId, documentBodyId);
            }
            return false;
        }

        public bool CanCreateNewVersion(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            
            var currentVersion = DocumentBodyRepository.GetCurrentVersion(documentId);
            
            if (currentVersion == null)
            {
                return true;
            }
            
            if (currentVersion.IsReviewed)
            {
                return true;
            }
            
            return false;
        }

        public DocumentBody RollbackToPreviousDocumentBody(
            Guid documentId,
            Guid documentBodyId
            )
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId tidak boleh kosong");
            }
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong");
            }
            var documentBody = DocumentBodyRepository.GetDocumentBodyById(documentId, documentBodyId);
            if (documentBody != null)
            {
                var currentVersion = DocumentBodyRepository.GetCurrentVersion(documentId);
                if (currentVersion != null)
                {
                    currentVersion.IsCurrentVersion = false;
                }
                documentBody.IsCurrentVersion = true;
                return documentBody;
            }
            throw new Exception("Document body not found");
        }
    }
}
