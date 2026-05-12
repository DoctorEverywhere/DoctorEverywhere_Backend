using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorEverywhere.Controllers
{

    [Route("api/[controller]")] //api/review
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("{doctorId}")]
        public async Task<IActionResult> CreateReview(int doctorId, [FromBody] CreateReviewDto reviewDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _reviewService.CreateReview(userId, doctorId, reviewDto);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Doctor, Patient, Manager")]
        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetReviews(int doctorId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByDoctorId(doctorId);
                return StatusCode(StatusCodes.Status200OK, reviews);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
