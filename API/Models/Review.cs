using API.Helpers.Enums;
using API.Helpers.ExtraClass;
using API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Review
    {
        [Key, Required]
        public Guid Id { get; set; }

        public string? Comment { get; set; }

        [Required]
        public Guid FK_DocumentBodyId { get; set; }
        public DocumentBody? DocumentBody { get; set; }

        [Required]
        public Guid FK_UserLecturerId { get; set; }
        public User? UserLecturer { get; set; }

        [Required]
        public Guid FK_UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public ReviewStatus Status { get; set; } = ReviewStatus.NeedsRevision;

        public ReviewState? State { get; set; } // Current state of the review process

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; } = DateTime.Now;

        public virtual List<ReviewRepository> Reviews { get; set; } = []; // Collection of reviews for this request, equal to 'new List<ReviewRepository>()'
    }
}