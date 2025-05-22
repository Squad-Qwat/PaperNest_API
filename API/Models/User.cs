using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class User
    {
        [Key, Required]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(12)]
        public string Password { get; set; }

        [MaxLength(15)]
        public string Username { get; set; }

        // one to many
        public ICollection<UserWorkspace> UserWorkspace = new List<UserWorkspace>();

        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; }
    }
}
