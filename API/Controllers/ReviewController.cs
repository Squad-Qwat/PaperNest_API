using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Helpers.Enums;

namespace API.Controllers
{
    [ApiController, Route("api/review")]
    public class ReviewController : Controller
    {
        private readonly ReviewService _reviewService;
        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("{documentBodyId}")]
        public IActionResult GetReviews(Guid documentBodyId)
        {

            var reviews = _reviewService.GetReviewByDocumentBodyId(documentBodyId);
            return Ok(reviews);
        }

        [HttpPost("{documentBodyId}/{UserLecturerId}")]
        public IActionResult AddReview(Guid documentBodyId, Guid UserLecturerId, [FromBody] string comment,
            [FromQuery] ReviewStatus status
            )
        {
            _reviewService.AddReview(documentBodyId, UserLecturerId, comment, status);
            return CreatedAtAction(nameof(GetReviews), new { documentBodyId }, null);
        }

        [HttpDelete("{reviewId}")]
        public IActionResult RemoveReview(Guid reviewId)
        {
            var review = _reviewService.GetReviewById(reviewId);
            if (review == null)
            {
                return NotFound(new
                {
                    message = "Review tidak ditemukan"
                });
            }
            var isRemoved = _reviewService.RemoveReview(reviewId);
            if (isRemoved)
            {
                return Ok(new
                {
                    message = "Sukses menghapus review"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = "Gagal menghapus review"
                });
            }
        }
    }
}
