using API.Models;
using API.Repositories;

namespace API.Services
{
    public class DocumentService
    {
        public static void Create(Document document)
        {
            DocumentRepository.documentRepository.Add(document);
        }

        public static IEnumerable<Document> GetAll()
        {
            return DocumentRepository.documentRepository;
        }

        public static Document? GetById(Guid documentId)
        {
            return DocumentRepository.documentRepository.FirstOrDefault(d => d.Id == documentId);
        }

        public static IEnumerable<Document> GetByUserId(Guid userId)
        {
            return DocumentRepository.documentRepository
                .Where(d => d.Workspace?.UserWorkspaces?.Any(uw => uw.User?.Id == userId)
                   ?? false);
        }

        public static IEnumerable<Document> GetByWorkspaceId(Guid workspaceId)
        {
            return DocumentRepository.documentRepository.Where(d => d.Workspace?.Id == workspaceId);
        }

        public static void Update(Guid id, Document document)
        {
            var existingDocument = GetById(id);

            if (existingDocument != null)
            {
                existingDocument.Title = document.Title;
                existingDocument.SavedContent = document.SavedContent;

                existingDocument.UpdateAt = DateTime.Now;
            }
        }

        public static void Delete(Guid id)
        {
            var existingDocument = GetById(id);

            if (existingDocument != null)
            {
                DocumentRepository.documentRepository.Remove(existingDocument);
            }
        }
    }
}