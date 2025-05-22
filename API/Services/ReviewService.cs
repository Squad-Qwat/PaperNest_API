using API.Helpers.Enums;
using API.Models;
using API.Repositories;
// using System.Diagnostics;

namespace API.Services
{
    public class ReviewService
    {
        public List<Review> GetAllReviews()
        {
            try
            {
                if (ReviewRepository.Reviews == null)
                {
                    throw new InvalidOperationException("Tidak ada data review yang tersedia");
                }
                if (ReviewRepository.Reviews.Count == 0)
                {
                    throw new InvalidOperationException("Tidak ada data review yang tersedia");
                }
                return ReviewRepository.Reviews;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error retrieving reviews: {ex.Message}");
                return []; // Setara dengan 'Enumerable.Empty<Review>()'
            }
        }
        public Review? GetReviewById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Id tidak boleh kosong", nameof(id));
                }

                return ReviewRepository.GetReviewById(id);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving review by ID: {ex.Message}");
                return null; // Setara dengan 'default(Review?)'
            }
        }
        public void AddReview(
            Guid documentBodyId,
            Guid userId,
            string comment,
            ReviewStatus status
            )
        {
            try
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
                if (!Enum.IsDefined(typeof(ReviewStatus), status))
                {
                    throw new ArgumentException("Status tidak valid", nameof(status));
                }
                ReviewRepository.AddReview(documentBodyId, userId, comment, status);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error adding review: {ex.Message}");
                return; // Rethrow the exception to be handled by the caller
            }
        }

        public Review? GetReviewByDocumentBodyId(Guid documentBodyId)
        {
            try
            {
                if (documentBodyId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentBodyId tidak boleh kosong", nameof(documentBodyId));
                }
                var review = ReviewRepository.GetReviewByDocumentBodyId(documentBodyId) ??
                    throw new InvalidOperationException("Review tidak ditemukan untuk DocumentBodyId yang diberikan");
                /*
                 * Setara dengan:
                    if (review == null)
                    {
                        throw new InvalidOperationException("Review tidak ditemukan untuk DocumentBodyId yang diberikan");
                    }
                */
                return review;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error retrieving review by DocumentBodyId: {ex.Message}");
                return null; // Setara dengan 'default(Review?)'
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error retrieving review by DocumentBodyId: {ex.Message}");
                return null; // Setara dengan 'default(Review?)'
            }
        }

        /*
        public Review? UpdateReview(
            Guid reviewId,
            string? comment = null,
            ReviewStatus? status = null
            )
        {
            try
            {
                if (reviewId == Guid.Empty)
                {
                    throw new ArgumentException("ReviewId tidak boleh kosong", nameof(reviewId));
                }
                var review = ReviewRepository.UpdateReview(reviewId, comment, status) ??
                    throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
                
                 * Setara dengan:
                 * if (review == null)
                 * {
                 *    throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
                 * }
                
                return review;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating review: {ex.Message}");
                return null; // Setara dengan 'default(Review?)'
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating review: {ex.Message}");
                return null; // Setara dengan 'default(Review?)'
            }
        }
        */

        public bool UpdateReview(
            Guid reviewId,
            string? comment = null,
            ReviewStatus? status = null
            )
        {
            try
            {
                if (reviewId == Guid.Empty)
                {
                    throw new ArgumentException("ReviewId tidak boleh kosong", nameof(reviewId));
                }

                if (status.HasValue && !Enum.IsDefined(typeof(ReviewStatus), status.Value))
                {
                    throw new ArgumentException("Status tidak valid", nameof(status));
                }

                if (comment == null)
                {
                    throw new ArgumentException("Comment tidak boleh null", nameof(comment));
                }

                if (string.IsNullOrWhiteSpace(comment))
                {
                    throw new ArgumentException("Comment tidak boleh kosong", nameof(comment));
                }
                if (status == null)
                {
                    throw new ArgumentException("Status tidak boleh null", nameof(status));
                }

                if (!Enum.IsDefined(typeof(ReviewStatus), status))
                {
                    throw new ArgumentException("Status tidak valid", nameof(status));
                }

                var review = ReviewRepository.UpdateReview(reviewId, comment, status);

                if(!review)
                {
                    throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
                }

                /*
                 * Opsional:
                 * if (review == null)
                 * {
                 *    throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
                 * }
                */

                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating review: {ex.Message}");
                return false; // Setara dengan 'default(bool)'
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating review: {ex.Message}");
                return false; // Setara dengan 'default(bool)'
            }
            /* Opsional:
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error updating review: {ex.Message}");
                return false; // Setara dengan 'default(bool)'
            }
            */
        }

        public bool RemoveReview(Guid reviewId)
        {
            try
            {
                if (reviewId == Guid.Empty)
                {
                    throw new ArgumentException("ReviewId tidak boleh kosong", nameof(reviewId));
                }

                var review = ReviewRepository.GetReviewById(reviewId) ??
                    throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
                /*
                 * Setara dengan:
                    if (review == null)
                    {
                        throw new InvalidOperationException("Review tidak ditemukan untuk ReviewId yang diberikan");
                    }
                */
                ReviewRepository.RemoveReview(reviewId);
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error removing review: {ex.Message}");
                return false; // Setara dengan 'default(bool)'
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error removing review: {ex.Message}");
                return false; // Setara dengan 'default(bool)'
            }
        }
    }
}