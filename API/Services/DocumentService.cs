using API.Models;
using API.Repositories;

namespace API.Services
{
    public class DocumentService
    {
        public void Create(Document document)
        {
            DocumentRepository.documentRepository.Add(document);
        }
        
        public IEnumerable<Document> GetAll()
        {
            return DocumentRepository.documentRepository;
        }

        public Document? GetById(Guid documentId)
        {
            return DocumentRepository.documentRepository.FirstOrDefault(d => d.Id == documentId);
        }

        public IEnumerable<Document> GetByUserId(Guid userId)
        {
            // Dapatkan semua workspace yang dimiliki user
            var userWorkspaces = UserWorkspaceRepository.GetUserWorkspacesByUserId(userId);
            var workspaceIds = userWorkspaces.Select(uw => uw.FK_WorkspaceId).ToList();
            
            // Kembalikan semua dokumen yang berada di workspace-workspace tersebut
            return DocumentRepository.documentRepository
                .Where(d => workspaceIds.Contains(d.FK_WorkspaceId));
        }

        public IEnumerable<Document> GetByWorkspaceId(Guid workspaceId)
        {
            return DocumentRepository.documentRepository.Where(d => d.FK_WorkspaceId == workspaceId);
        }

        public void Update(Guid id, Document document)
        {
            var existingDocument = GetById(id);

            if (existingDocument != null)
            {
                existingDocument.Title = document.Title;
                existingDocument.SavedContent = document.SavedContent;
                
                existingDocument.UpdateAt = DateTime.Now;
            }
        }

        public void Delete(Guid deletedDocumentId)
        {
            var existingDocument = GetById(deletedDocumentId);

            if (existingDocument != null)
            {
                DocumentRepository.documentRepository.Remove(existingDocument);
            }
        }
    }
}
