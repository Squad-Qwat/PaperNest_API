using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Helpers.Enums;

namespace API.Controllers
{
    [ApiController, Route("api/review")]
    public class ReviewController(ReviewService reviewService) : Controller
    {
        private readonly ReviewService _reviewService = reviewService;

        /*
         * Setara dengan:
         * public ReviewController(ReviewService reviewService)
         * {
         *   _reviewService = reviewService;
         * }
         */

        [HttpGet("{documentBodyId}")]
        public IActionResult GetReviews(Guid documentBodyId)
        {
            if (documentBodyId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "DocumentBodyId tidak boleh kosong"
                });
            }

            var reviews = _reviewService.GetReviewByDocumentBodyId(documentBodyId);

            if (reviews == null)
            {
                return NotFound(new
                {
                    message = "Review tidak ditemukan untuk DocumentBodyId yang diberikan"
                });
            }

            if (reviews.FK_DocumentBodyId.Equals(0) || reviews.Reviews.Count == 0)
            {
                return NotFound(new
                {
                    message = "Tidak ada review untuk DocumentBodyId yang diberikan"
                });
            }

            return Ok(reviews);
        }

        [HttpPost("{documentBodyId}/{UserLecturerId}")]
        public IActionResult AddReview(Guid documentBodyId, Guid UserLecturerId, [FromBody] string comment, [FromQuery] ReviewStatus status)
        {
            // Validate input parameters
            if (documentBodyId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "DocumentBodyId tidak boleh kosong"
                });
            }

            if (UserLecturerId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "UserLecturerId tidak boleh kosong"
                });
            }
            
            if (string.IsNullOrWhiteSpace(comment))
            {
                return BadRequest(new
                {
                    message = "Comment tidak boleh kosong"
                });
            }

            if (!Enum.IsDefined(typeof(ReviewStatus), status))
            {
                return BadRequest(new
                {
                    message = "Status tidak valid"
                });
            }

            _reviewService.AddReview(documentBodyId, UserLecturerId, comment, status);
            return CreatedAtAction(nameof(GetReviews), new { documentBodyId }, null);
        }

        [HttpPut("{reviewId}")]
        public IActionResult UpdateReview(Guid reviewId, [FromBody] string comment, [FromQuery] ReviewStatus status)
        {
            if (reviewId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "ReviewId tidak boleh kosong"
                });
            }

            var review = _reviewService.GetReviewById(reviewId);
            if (review == null)
            {
                return NotFound(new
                {
                    message = "Review tidak ditemukan"
                });
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                return BadRequest(new
                {
                    message = "Comment tidak boleh kosong"
                });
            }
            if (!Enum.IsDefined(typeof(ReviewStatus), status))
            {
                return BadRequest(new
                {
                    message = "Status tidak valid"
                });
            }
            review.Comment = comment;
            review.Status = status;
            review.UpdateAt = DateTime.Now;
            var isUpdated = _reviewService.UpdateReview(reviewId, review.Comment, review.Status);
            if (!isUpdated)
            {
                return BadRequest(new
                {
                    message = "Gagal memperbarui review"
                });
            }
            return Ok(new
            {
                message = "Sukses memperbarui review"
            });
        }

        [HttpDelete("{reviewId}")]
        public IActionResult RemoveReview(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "ReviewId tidak boleh kosong"
                });
            }

            var review = _reviewService.GetReviewById(reviewId);
            if (review == null)
            {
                return NotFound(new
                {
                    message = "Review tidak ditemukan"
                });
            }

            var isRemoved = _reviewService.RemoveReview(reviewId);
            if (!isRemoved)
            {
                return BadRequest(new
                {
                    message = "Gagal menghapus review"
                });
            }

            return Ok(new
            {
                message = "Sukses menghapus review"
            });
        }
    }
}