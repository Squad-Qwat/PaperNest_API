using API.Models;
// using System.Diagnostics;

namespace API.Repositories
{
    public class DocumentBodyRepository
    {
        private static readonly List<DocumentBody> _documentBodies = []; // Setara dengan 'new List<DocumentBody>();
        private static DocumentBody? documentBodyForService;

        public DocumentBodyRepository()
        {
            documentBodyForService = new(); // Setara dengan 'new DocumentBody();'
        }

        public DocumentBodyRepository(Guid id, string content, Guid documentId, bool isCurrentVersion)
        {
            documentBodyForService = new()
            {
                Id = id,
                Content = content,
                FK_DocumentId = documentId,
                IsCurrentVersion = isCurrentVersion
            }; // Setara dengan 'new DocumentBody();'
        }

        public DocumentBodyRepository(Guid id, string content, Guid documentId, bool isCurrentVersion, bool isReviewed, DateTime createdAt)
        {
            documentBodyForService = new()
            {
                Id = id,
                Content = content,
                FK_DocumentId = documentId,
                IsCurrentVersion = isCurrentVersion,
                IsReviewed = isReviewed,
                CreatedAt = createdAt
            }; // Setara dengan 'new DocumentBody();'
        }

        public static List<DocumentBody> DocumentBodies
        {
            get => _documentBodies;
            set
            {
                try
                {
                    if (value == null)
                    {
                        Console.WriteLine("Setting new document body list.");
                        return;
                    }

                    if (value.Count == 0)
                    {
                        Console.WriteLine("Setting empty document body list.");
                    }
                    else
                    {
                        Console.WriteLine($"Setting document body list with {value.Count} items.");
                    }

                    if (value.Any(c => c == null))
                    {
                        throw new ArgumentException("Document body list cannot contain null items.");
                    }

                    if (value.Any(c => c.Id == Guid.Empty))
                    {
                        throw new ArgumentException("Document body list cannot contain items with empty IDs.");
                    }
                    _documentBodies.Clear(); // Clear existing data
                    _documentBodies.AddRange(value); // Add new data
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error setting document body: {ex.Message}");
                }
            }
        }

        public static DocumentBody? GetDocumentBodyById(Guid documentId, Guid documentBodyId)
        {
            try
            {
                if (documentBodyId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentBodyId tidak boleh kosong");
                }
                return _documentBodies.FirstOrDefault(db => db.Id == documentBodyId && db.FK_DocumentId == documentId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving DocumentBody: {ex.Message}");
                return null;
            }
        }

        public static void AddDocumentBody(DocumentBody documentBody)
        {
            if (documentBody == null)
            {
                Console.WriteLine("DocumentBody tidak boleh null");
                return; // Tidak menambahkan null
            }
            _documentBodies.Add(documentBody);
        }

        public static IEnumerable<DocumentBody> GetDocumentBodiesByDocumentId(Guid documentId)
        {
            // Mengambil semua DocumentBody yang terkait dengan documentId tertentu
            if (documentId == Guid.Empty)
            {
                Console.WriteLine("DocumentId tidak boleh kosong");
                return []; // Setara dengan 'Enumerable.Empty<DocumentBody>()'
            }
            
            return _documentBodies.Where(db => db.FK_DocumentId == documentId)
                .OrderByDescending(db => db.IsCurrentVersion).ThenByDescending(db => db.CreatedAt);
        }

        public static DocumentBody? GetCurrentVersion(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                Console.WriteLine("DocumentId tidak boleh kosong");
                return null; // Tidak ada versi terkini jika documentId kosong
            }

            return _documentBodies.FirstOrDefault(db => db.FK_DocumentId == documentId && db.IsCurrentVersion);
        }

        public static DocumentBody? GetBeforeCurrentVersion(Guid documentId)
        {
            if (documentId == Guid.Empty)
            {
                Console.WriteLine("DocumentId tidak boleh kosong");
                return null; // Tidak ada versi sebelumnya jika documentId kosong
            }

            return _documentBodies
                .Where(db => db.FK_DocumentId == documentId && !db.IsCurrentVersion)
                .OrderByDescending(db => db.CreatedAt)
                .FirstOrDefault();
        }

        public static bool DeleteDocumentBody(Guid documentId, Guid documentBodyId)
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

                var documentBody = GetDocumentBodyById(documentId, documentBodyId) ?? 
                    throw new ArgumentException("DocumentBody tidak ditemukan");
                /*
                 * Setara dengan:
                 * 'if (documentBody == null)
                 * {
                 *    throw new ArgumentException("DocumentBody tidak ditemukan");
                 * }'
                 */
                _documentBodies.Remove(documentBody);
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error deleting DocumentBody: {ex.Message}");
                return false; // Mengembalikan false jika terjadi kesalahan
            }
        }

        //UnitTest only
        public static void ClearAllDocumentBodies()
        {
            _documentBodies.Clear();
        }
    }
}