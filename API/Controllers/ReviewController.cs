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
    }
}
