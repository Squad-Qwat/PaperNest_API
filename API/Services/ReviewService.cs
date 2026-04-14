using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using System.Diagnostics;

namespace API.Services
{
    public class ReviewService
    {
        public List<Review> GetAllReviews()
        {
            return ReviewRepository.Reviews;
        }
        public Review? GetReviewById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id tidak boleh kosong", nameof(id));
            }

            return ReviewRepository.GetReviewById(id);
        }
        public void AddReview(
            Guid documentBodyId,
            Guid userId,
            string comment,
            ReviewStatus status
            )
        {
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong", nameof(documentBodyId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId tidak boleh kosong", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Comment tidak boleh kosong", nameof(comment));
            }
            ReviewRepository.AddReview(documentBodyId, userId, comment, status);
        }

        public Review? GetReviewByDocumentBodyId(Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                throw new ArgumentException("DocumentBodyId tidak boleh kosong", nameof(documentBodyId));
            }
            var review = ReviewRepository.GetReviewByDocumentBodyId(documentBodyId);
            return review; // Kembalikan null jika review tidak ditemukan daripada melempar exception
        }

        public bool RemoveReview(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw new ArgumentException("ReviewId tidak boleh kosong", nameof(reviewId));
            }

            var review = ReviewRepository.GetReviewById(reviewId);
            
            if (review == null)
            {
                throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
            }
            ReviewRepository.RemoveReview(reviewId);
            return true;
            
        }
    }
}
