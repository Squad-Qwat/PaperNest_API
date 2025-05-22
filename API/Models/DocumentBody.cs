using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace API.Models
{
    public class DocumentBody
    {
        [Key, Required]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        [Required]
        public string? Content { get; set; }

        [Required]
        public Guid FK_DocumentId { get; set; }

        public Document Document { get; set; }

        [Required]
        public bool IsCurrentVersion { get; set; }

        public bool IsReviewed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

        // Constructor for EF
        public DocumentBody() { }

        public DocumentBody(string content, Guid documentId)
        {
            Content = content;
            FK_DocumentId = documentId;
            IsCurrentVersion = true; // By default, newly created is current
        }
    }
}
