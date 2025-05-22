using API.Helpers.Enums;
using API.Models;

namespace API.Repositories
{
    public class ReviewRepository
    {
        private static readonly List<Review> _reviews = new List<Review>();
        public static List<Review> Reviews
        {
            get => _reviews;
            set
            {
                if (value != null)
                {
                    _reviews.Clear();
                    _reviews.AddRange(value);
                }
            }
        }
        public static Review? GetReviewById(Guid id)
        {
            return _reviews.FirstOrDefault(r => r.Id == id);
        }
        public static void AddReview(Guid documentBodyId, Guid userLecturerId, string comment, ReviewStatus status)
        {
            var review = new Review
            {
                FK_DocumentBodyId = documentBodyId,
                FK_UserLecturerId = userLecturerId,
                Comment = comment,
                Status = status
            };
            _reviews.Add(review);
        }

        public static Review? GetReviewByDocumentBodyId(
            Guid documentBodyId)
        {
            return _reviews.FirstOrDefault(r => r.FK_DocumentBodyId == documentBodyId);
        }

        public static bool RemoveReview(Guid reviewId)
        {
            var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                _reviews.Remove(review);
                return true;
            }
            return false;
        }
    }
}
