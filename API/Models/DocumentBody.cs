using System.ComponentModel.DataAnnotations;

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

        public Document? Document { get; set; }

        public string? Comment { get; set; }

        [Required]
        public Guid FK_UserCreaotorId { get; set; }
        public User? UserCreator { get; set; }

        [Required]
        public bool IsCurrentVersion { get; set; }

        public bool IsReviewed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;
    }
}