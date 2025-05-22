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

        public static DocumentBody? GetDocumentBodyById(Guid documentId,Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong");
            }
            return _documentBodies.FirstOrDefault(db => db.Id == documentBodyId && db.FK_DocumentId == documentId);
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

        public static bool DeleteDocumentBody(Guid documentId,Guid documentBodyId)
        {
            var documentBody = GetDocumentBodyById(documentId, documentBodyId);
            if (documentBody != null)
            {
                _documentBodies.Remove(documentBody);
                return true;
            }
            return false;
        }


        //UnitTest only
        public static void ClearAllDocumentBodies()
        {
            _documentBodies.Clear();
        }

    }
}
