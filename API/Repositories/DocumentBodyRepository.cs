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

        public static DocumentBody? GetDocumentBodyById(Guid id)
        {
            return _documentBodies.FirstOrDefault(db => db.Id == id);
        }

        public static void AddDocumentBody(DocumentBody documentBody)
        {
            if (documentBody != null)
            {
                _documentBodies.Add(documentBody);
            }
        }
    }
}
