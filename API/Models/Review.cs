using API.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Review
    {
        [Key, Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Comment { get; set; } = string.Empty;

        [Required]
        public Guid FK_DocumentBodyId { get; set; }
        public DocumentBody DocumentBody { get; set; }

        [Required]
        public Guid FK_UserId { get; set; }
        public User User { get; set; }

        [Required]
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

    }
}
