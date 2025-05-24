using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Helpers.Enums;

namespace API.Controllers
{
    [ApiController,Route("api/review")]
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
            try
            {
                var reviews = _reviewService.GetReviewByDocumentBodyId(documentBodyId);
                if (reviews == null)
                {
                    return NotFound(new
                    {
                        message = "Review tidak ditemukan"
                    });
                }
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil review" });
            }
        }

        [HttpGet("{documentBodyId}/{reviewId}")]
        public IActionResult GetReviewById(Guid documentBodyId, Guid reviewId)
        {
            try
            {
                var review = _reviewService.GetReviewById(reviewId);
                if (review == null)
                {
                    return NotFound(new
                    {
                        message = "Review tidak ditemukan"
                    });
                }
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil review" });
            }
        }

        [HttpPost("{documentBodyId}/{UserLecturerId}")]
        public IActionResult AddReview(Guid documentBodyId, Guid UserLecturerId, [FromBody] string comment,
            [FromQuery] ReviewStatus status 
            )
        {
            try
            {
                _reviewService.AddReview(documentBodyId, UserLecturerId, comment, status);
                return CreatedAtAction(nameof(GetReviews), new { documentBodyId }, null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menambah review" });
            }
        }

        [HttpDelete("{reviewId}")]
        public IActionResult RemoveReview(Guid reviewId)
        {
            try
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus review" });
            }
        }
    }
}
