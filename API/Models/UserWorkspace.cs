using API.Helpers.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class UserWorkspace
    {
        [Key]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        [Required]
        public WorkspaceRole WorkspaceRole { get; set; } = WorkspaceRole.Member;

        public Guid FK_UserId { get; set; }

        public User? User { get; set; }

        public Guid FK_WorkspaceId { get; set; }

        public Workspace? Workspace { get; set; }

        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; }
    }
}