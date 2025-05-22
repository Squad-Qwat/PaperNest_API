using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using API.Repositories;
using API.Helpers.ExtraClass;

namespace API.Models
{
    public class ReviewRequest
    {
        [Key, Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? Title { get; set; } // Title of the submission

        [Required]
        public string? Abstract { get; set; } // Abstract of the submission

        [Required]
        public string? ResearcherName { get; set; } // Submitter's name

        public DateTime SubmissionDate { get; private set; } // When it was submitted
        public ReviewState? State { get; set; } // Current state of the review process
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        public Guid UserId { get; set; } // User who submitted this request

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        // Link to the 'local' Document this request is about
        [Required]
        public Guid DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; } = null!;

        // Link to the specific DocumentBody being proposed for review (the content of this submission)
        [Required]
        public Guid DocumentBodyId { get; set; } // This is the ID of the DocumentBody that is being submitted

        [ForeignKey("DocumentBodyId")]
        public virtual DocumentBody DocumentBody { get; set; } = null!; // The proposed content version

        public virtual List<ReviewRepository> Reviews { get; private set; } = []; // Collection of reviews for this request, equal to 'new List<ReviewRepository>()'
    }
}