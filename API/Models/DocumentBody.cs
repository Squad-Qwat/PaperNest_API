using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace API.Models
{
    public class DocumentBody
    {
        [Key, Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? Content { get; set; }

        [Required]
        public Guid FK_DocumentId { get; set; }

        public Document? Document { get; set; }

        [Required]
        public bool IsCurrentVersion { get; set; } = true;

        public bool IsReviewed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
