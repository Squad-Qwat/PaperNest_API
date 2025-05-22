using API.Models;
using API.Repositories;

namespace API.Services
{
    public class DocumentService
    {
        public void Create(Document document)
        {
            try
            {
                if (document == null)
                {
                    throw new ArgumentNullException(nameof(document), "Document cannot be null");
                }
                DocumentRepository.documentRepository.Add(document);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error creating document: {ex.Message}");
            }
        }

        public IEnumerable<Document> GetAll()
        {
            if(DocumentRepository.documentRepository.Count == 0)
            {
                Console.WriteLine("No documents found.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            if (DocumentRepository.documentRepository == null)
            {
                Console.WriteLine("Document repository is not initialized.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            return DocumentRepository.documentRepository;
        }

        public Document? GetById(Guid documentId)
        {
            if(documentId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve document with empty ID.");
                return null;
            }

            if (DocumentRepository.documentRepository == null)
            {
                Console.WriteLine("Document repository is not initialized.");
                return null;
            }

            if (DocumentRepository.documentRepository.Count == 0)
            {
                Console.WriteLine("No documents found.");
                return null;
            }

            return DocumentRepository.documentRepository.FirstOrDefault(d => d.Id == documentId);
        }

        public IEnumerable<Document> GetByUserId(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve documents for user with empty ID.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            if (DocumentRepository.documentRepository == null)
            {
                Console.WriteLine("Document repository is not initialized.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            if (DocumentRepository.documentRepository.Count == 0)
            {
                Console.WriteLine("No documents found.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            // Dapatkan semua workspace yang dimiliki user
            var userWorkspaces = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            if (userWorkspaces == null || !userWorkspaces.Any())
            {
                Console.WriteLine("No workspaces found for the user.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            var workspaceIds = userWorkspaces.Select(uw => uw.FK_WorkspaceId).ToList();
            if (workspaceIds.Count == 0)
            {
                Console.WriteLine("No workspaces found for the user.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            // Kembalikan semua dokumen yang berada di workspace-workspace tersebut
            return DocumentRepository.documentRepository
                .Where(d => workspaceIds.Contains(d.FK_WorkspaceId));
        }

        public IEnumerable<Document> GetByWorkspaceId(Guid workspaceId)
        {
            if(workspaceId == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve documents for workspace with empty ID.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            if (DocumentRepository.documentRepository == null)
            {
                Console.WriteLine("Document repository is not initialized.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            if (DocumentRepository.documentRepository.Count == 0)
            {
                Console.WriteLine("No documents found.");
                return []; // Setara dengan 'Enumerable.Empty<Document>()'
            }

            return DocumentRepository.documentRepository.Where(d => d.FK_WorkspaceId == workspaceId);
        }

        public void Update(Guid id, Document document)
        {
            if(id == Guid.Empty)
            {
                Console.WriteLine("Cannot update document with empty ID.");
                return;
            }

            if (document == null)
            {
                Console.WriteLine("Cannot update document with null value.");
                return;
            }

            var existingDocument = GetById(id);

            if (existingDocument == null)
            {
                Console.WriteLine($"Document with ID {id} not found.");
                return;
            }
            existingDocument.Title = document.Title;
            existingDocument.SavedContent = document.SavedContent;
            existingDocument.UpdateAt = DateTime.Now;
        }

        public void Delete(Guid deletedDocumentId)
        {
            if(deletedDocumentId == Guid.Empty)
            {
                Console.WriteLine("Cannot delete document with empty ID.");
                return;
            }

            if (DocumentRepository.documentRepository == null)
            {
                Console.WriteLine("Document repository is not initialized.");
                return;
            }

            if (DocumentRepository.documentRepository.Count == 0)
            {
                Console.WriteLine("No documents found.");
                return;
            }

            var existingDocument = GetById(deletedDocumentId);

            if (existingDocument == null)
            {
                Console.WriteLine($"Document with ID {deletedDocumentId} not found.");
                return;
            }
            
            DocumentRepository.documentRepository.Remove(existingDocument);
        }
    }
}