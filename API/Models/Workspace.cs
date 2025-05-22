using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Workspace
    {
        [Key, Required]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public ICollection<Document> Documents { get; set; } = []; // Setara dengan 'new List<Document>'

        public ICollection<UserWorkspace> UserWorkspaces { get; set; } = []; // Setara dengan 'new List<UserWorkspace>'

        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; }
    }
}
