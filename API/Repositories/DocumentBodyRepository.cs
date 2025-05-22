using API.Models;

namespace API.Repositories
{
    public class DocumentBodyRepository
    {
        private static readonly List<DocumentBody> _documentBodies = new List<DocumentBody>();

        public static List<DocumentBody> DocumentBodies
        {
            get => _documentBodies;
            set
            {
                if (value != null)
                {
                    _documentBodies.Clear();
                    _documentBodies.AddRange(value);
                }
            }
        }

        public static DocumentBody? GetDocumentBodyById(Guid documentBodyId)
        {
            return _documentBodies.FirstOrDefault(db => db.Id == documentBodyId);
        }

        public static void AddDocumentBody(DocumentBody documentBody)
        {
            if (documentBody != null)
            {
                _documentBodies.Add(documentBody);
            }
        }

        public static IEnumerable<DocumentBody> GetDocumentBodiesByDocumentId(Guid documentId)
        {
            return _documentBodies.Where(db => db.FK_DocumentId == documentId)
                .OrderByDescending(db => db.IsCurrentVersion).ThenByDescending(db => db.CreatedAt);
        }

        public static DocumentBody? GetCurrentVersion(Guid documentId)
        {
            return _documentBodies.FirstOrDefault(db => db.FK_DocumentId == documentId && db.IsCurrentVersion);
        }

        public static DocumentBody? GetBeforeCurrentVersion(Guid documentId)
        {
            return _documentBodies
                .Where(db => db.FK_DocumentId == documentId && !db.IsCurrentVersion)
                .OrderByDescending(db => db.CreatedAt)
                .FirstOrDefault();
        }

        //UnitTest only
        public static void ClearAllDocumentBodies()
        {
            _documentBodies.Clear();
        }

    }
}
