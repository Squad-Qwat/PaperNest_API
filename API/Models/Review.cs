using API.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Review
    {
        [Key, Required]
        public Guid Id { get; set; }

        public string Comment { get; set; }

        [Required]
        public Guid FK_DocumentBodyId { get; set; }
        public DocumentBody DocumentBody { get; set; }

        [Required]
        public Guid FK_UserId { get; set; }
        public User User { get; set; }

        [Required]
        public ReviewStatus Status { get; set; } = ReviewStatus.NeedsRevision;

        [Required]
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; } = DateTime.Now;

        public Review() { }

        public Review(Guid id, ReviewStatus status,Guid documentBodyId, Guid userId, string comment)
        {
            Id = id;
            Status = status;
            Comment = comment;
            FK_DocumentBodyId = documentBodyId;
            FK_UserId = userId;
        }
    }
}
