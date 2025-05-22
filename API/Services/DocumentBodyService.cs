using API.Models;
using API.Repositories;
// using System.Diagnostics;

namespace API.Services
{
    public class DocumentBodyService
    {
        public List<DocumentBody> GetAllDocumentBodies()
        {
            try
            {
                if (DocumentBodyRepository.DocumentBodies == null)
                {
                    throw new Exception("Document bodies not found");
                }
                return DocumentBodyRepository.DocumentBodies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving document bodies: {ex.Message}");
                return []; // Setara dengan 'new List<DocumentBody>();'
            }
        }

        public DocumentBody? GetDocumentBodyById(Guid documentId, Guid documentBodyId)
        {
            try
            {
                if (documentBodyId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentBodyId tidak boleh kosong");
                }

                return DocumentBodyRepository.GetDocumentBodyById(documentId, documentBodyId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving document body by ID: {ex.Message}");
                return null; // Melempar ulang exception untuk penanganan lebih lanjut
            }
        }

        public IEnumerable<DocumentBody> GetDocumentBodiesByDocumentId(Guid documentId)
        {
            try
            {
                if (documentId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentId tidak boleh kosong");
                }
                return DocumentBodyRepository.GetDocumentBodiesByDocumentId(documentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving document bodies by document ID: {ex.Message}");
                return []; // Setara dengan 'Enumberable.Empty<DocumentBody>;'
            }
        }

        public DocumentBody? CreateDocumentBody(
            Guid documentId,
            Guid userCreatorId,
            string comment,
            string content
            )
        {
            try
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
                    FK_UserCreatorId = userCreatorId,
                    IsCurrentVersion = isCurrentVersion
                };
                DocumentBodyRepository.AddDocumentBody(documentBody);
                return documentBody;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error creating document body: {ex.Message}");
                return null; // Melempar ulang exception untuk penanganan lebih lanjut
            }
        }

        public DocumentBody? GetCurrentVersion(Guid documentId)
        {
            try
            {
                if (documentId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentId tidak boleh kosong");
                }
                return DocumentBodyRepository.GetCurrentVersion(documentId);
            }
            catch (ArgumentException ex) 
            {
                Console.WriteLine($"Error retrieving current version: {ex.Message}");
                return null; // Melempar ulang exception untuk penanganan lebih lanjut
            }
        }

        public bool RemoveDocumentBody(Guid documentId, Guid documentBodyId)
        {
            try
            {
                if (documentId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentId tidak boleh kosong");
                }
                if (documentBodyId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentBodyId tidak boleh kosong");
                }
                var documentBody = DocumentBodyRepository.GetDocumentBodyById(documentId, documentBodyId) ??
                    throw new ArgumentException("Document body not found");
                /*
                 * Setara dengan:
                 * if (documentBody == null)
                 * {
                 *    throw new Exception("Document body not found");
                 * }
                 */

                // supaya hanya menghapus document body yang ada di dalam documentId yang sesuai
                return DocumentBodyRepository.DeleteDocumentBody(documentBody.FK_DocumentId, documentBody.Id);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error removing document body: {ex.Message}");
                return false; // Melempar ulang exception untuk penanganan lebih lanjut
            }
        }

        public bool CanCreateNewVersion(Guid documentId)
        {
            try
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

                if (!currentVersion.IsReviewed)
                {
                    throw new InvalidOperationException("Current version must be reviewed before creating a new version.");
                }

                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error checking if new version can be created: {ex.Message}");
                return false; // Melempar ulang exception untuk penanganan lebih lanjut
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error checking if new version can be created: {ex.Message}");
                return false; // Melempar ulang exception untuk penanganan lebih lanjut
            }
        }

        public DocumentBody? RollbackToPreviousDocumentBody(
            Guid documentId,
            Guid documentBodyId
            )
        {
            try
            {
                if (documentId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentId tidak boleh kosong");
                }
                if (documentBodyId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentBodyId tidak boleh kosong");
                }
                var documentBody = DocumentBodyRepository.GetDocumentBodyById(documentId, documentBodyId) ?? 
                    throw new ArgumentException("Document body not found");
                
                /*
                 * if (documentBody == null)
                 * {
                 *    throw new ArgumentException("Document body not found");
                 * }
                 */

                var currentVersion = DocumentBodyRepository.GetCurrentVersion(documentId);
                if (currentVersion != null)
                {
                    currentVersion.IsCurrentVersion = false;
                }

                documentBody.IsCurrentVersion = true;
                return documentBody;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error rolling back to previous document body: {ex.Message}");
                return null; // Melempar ulang exception untuk penanganan lebih lanjut
            }
        }
    }
}