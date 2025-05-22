using API.Helpers.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ReviewRequest
    {
        [Key, Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } // Title of the submission

        [Required]
        public string Abstract { get; set; } // Abstract of the submission

        [Required]
        public string ResearcherName { get; set; } // Submitter's name

        public DateTime SubmissionDate { get; private set; } // When it was submitted
        public ReviewState State { get; set; } // Current state of the review process
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

        public virtual List<Review> Reviews { get; private set; } = new List<Review>(); // Collection of reviews for this request

        // Constructor for Entity Framework
        protected ReviewRequest() { }

        public ReviewRequest(Guid id, string title, string abstractText, string researcherName, Guid userId, Guid documentId, Guid documentBodyId)
        {
            Id = id;
            Title = title;
            Abstract = abstractText;
            ResearcherName = researcherName;
            UserId = userId;
            DocumentId = documentId;
            DocumentBodyId = documentBodyId;
            SubmissionDate = DateTime.Now;
            State = new SubmittedState(); // Initial state
            CreatedAt = DateTime.Now;
            UpdateAt = DateTime.Now;
        }
    }
}
