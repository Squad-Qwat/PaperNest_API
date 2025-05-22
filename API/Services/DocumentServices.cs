using API.Models;

namespace API.Services
{
    public static class DocumentService
    {
        private static readonly List<Document> _documents = []; // Setara dengan 'new List<Document>()'
        private static readonly List<DocumentBody> _documentBodies = [];// Setara dengan 'new List<DocumentBody>()'

        public static List<Document> GetAll() => _documents;
        public static Document? GetById(Guid id) => _documents.FirstOrDefault(d => d.Id == id);
        public static List<Document> GetByUserId(Guid userId) => [.. _documents.Where(d => d.FK_UserId == userId)]; // Setara dengan '_documents.Where(d => d.FK_UserId == userId).ToList()'
        public static List<Document> GetByWorkspaceId(Guid workspaceId) => [.. _documents.Where(d => d.FK_WorkspaceId == workspaceId)]; // Setara dengan '_documents.Where(d => d.FK_WorkspaceId == workspaceId).ToList()'

        public static void Create(Document document)
        {
            document.Id = Guid.NewGuid(); // Ensure unique ID
            document.CreatedAt = DateTime.Now;
            document.UpdateAt = DateTime.Now;

            // When a new document is created, its initial content also forms a DocumentBody
            var initialDocumentBody = new DocumentBody(document.SavedContent ?? string.Empty, document.Id)
            {
                IsCurrentVersion = true // Tandai sebagai versi saat ini
            };
            _documentBodies.Add(initialDocumentBody);

            // document.CurrentDocumentBodyId = initialDocumentBody.Id; <- Link document to its initial body
            // document.HasDraft = false;  <- Initial content is now a formal version
            // document.LocalContentDraft = null; <- Clear local draft once versioned

            _documents.Add(document);
        }

        public static void Update(Guid id, Document updatedDocument)
        {
            var existingDocument = GetById(id);
            if (existingDocument != null)
            {
                existingDocument.Title = updatedDocument.Title;
                existingDocument.SavedContent = updatedDocument.SavedContent;
                existingDocument.User = updatedDocument.User;
                existingDocument.Workspace = updatedDocument.Workspace;
                existingDocument.CreatedAt = DateTime.Now;
                existingDocument.UpdateAt = DateTime.Now;
            }
        }

        public static void Delete(Guid id)
        {
            _documents.RemoveAll(d => d.Id == id);
            _documentBodies.RemoveAll(db => db.FK_DocumentId == id); // Also delete associated document bodies
        }

        // Sisanya sudah diurus di DocumentBodyService
    }
}