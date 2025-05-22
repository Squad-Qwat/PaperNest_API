using API.Helpers.Enums;
using API.Models;
using System.Xml.Linq;

namespace API.Repositories
{
    public class ReviewRepository
    {
        private static readonly List<Review> _reviews = []; // Setara dengan 'new List<Review>();'
        private static Review? reviewforservice;

        public ReviewRepository()
        {
            reviewforservice = new(); // Setara dengan 'new Review();'
        }

        public ReviewRepository(Guid id, ReviewStatus status, Guid documentBodyId, Guid userId, string comment)
        {
            reviewforservice = new()
            {
                Id = id,
                Status = status,
                Comment = comment,
                FK_DocumentBodyId = documentBodyId,
                FK_UserId = userId,
            }; // Setara dengan 'new Review();'
        }
        public static List<Review> Reviews
        {
            get => _reviews;
            set
            {
                try
                {
                    if (value == null)
                    {
                        Console.WriteLine("Setting new reviews list.");
                        return;
                    }

                    if (value.Count == 0)
                    {
                        Console.WriteLine("Setting empty reviews list.");
                    }
                    else
                    {
                        Console.WriteLine($"Setting reviews list with {value.Count} items.");
                    }

                    if (value.Any(c => c == null))
                    {
                        throw new ArgumentException("Reviews list cannot contain null items.");
                    }

                    if (value.Any(c => c.Id == Guid.Empty))
                    {
                        throw new ArgumentException("Reviews list cannot contain items with empty IDs.");
                    }
                    _reviews.Clear(); // Clear existing data
                    _reviews.AddRange(value); // Add new data
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error setting reviews: {ex.Message}");
                }
            }
        }
        public static Review? GetReviewById(Guid id)
        {
            if (id == Guid.Empty)
            {
                Console.WriteLine("Invalid ID provided.");
                return null;
            }
            if (_reviews == null)
            {
                Console.WriteLine("Reviews list is not initialized.");
                return null;
            }

            if (_reviews.Count == 0)
            {
                Console.WriteLine("No reviews available.");
                return null;
            }

            if (_reviews.Any(r => r.Id == Guid.Empty))
            {
                Console.WriteLine("Reviews list contains items with empty IDs.");
                return null;
            }
            
            if (_reviews.Any(r => r.Id == id) == false && _reviews.Count > 0)
            {
                Console.WriteLine($"No review found with ID: {id}");
                return null;
            }

            if (_reviews.Any(r => r.Id == id) == false)
            {
                Console.WriteLine($"No review found with ID: {id}");
                return null;
            }

            return _reviews.FirstOrDefault(r => r.Id == id);
        }
        public static void AddReview(Guid documentBodyId, Guid userId, string comment, ReviewStatus status)
        {
            if (documentBodyId == Guid.Empty || userId == Guid.Empty)
            {
                Console.WriteLine("Invalid document body ID or user ID provided.");
                return;
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                Console.WriteLine("Comment cannot be null or empty.");
                return;
            }

            if (!Enum.IsDefined(typeof(ReviewStatus), status))
            {
                Console.WriteLine("Invalid review status provided.");
                return;
            }

            var review = new Review
            {
                FK_DocumentBodyId = documentBodyId,
                FK_UserId = userId,
                Comment = comment,
                Status = status
            };
            _reviews.Add(review);
        }

        public static Review? GetReviewByDocumentBodyId(Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                Console.WriteLine("Invalid document body ID provided.");
                return null;
            }

            return _reviews.FirstOrDefault(r => r.FK_DocumentBodyId == documentBodyId);
        }

        public static bool RemoveReview(Guid reviewId)
        {
            try
            {
                if (reviewId == Guid.Empty)
                {
                    throw new ArgumentException("Review ID cannot be empty.", nameof(reviewId));
                }

                var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
                if (review == null)
                {
                    throw new KeyNotFoundException($"Review with ID {reviewId} not found.");
                }
                _reviews.Remove(review);
                return true;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}