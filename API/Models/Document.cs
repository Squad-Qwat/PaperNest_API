using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string? Title { get; set; }

        public string? SavedContent { get; set; }

        [Required]
        public Guid FK_WorkspaceId { get; set; }

        public Workspace? Workspace { get; set; }

        [Required]
        public Guid FK_UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; }
    }
}
