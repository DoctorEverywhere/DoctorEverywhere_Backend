using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DoctorEverywhere.Controllers
{

    [Route("api/[controller]")] //api/availability
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("slots")]
        public async Task<IActionResult> CreateorUpdateAvailability([FromBody] List<AvailabilityDto> availabilityDtos)
        {

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _availabilityService.CreateorUpdateAvailability(userId, availabilityDtos);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("slots")]
        public async Task<IActionResult> GetAvailability()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var availability = await _availabilityService.GetAvailability(userId);
                return StatusCode(StatusCodes.Status200OK, availability);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorAvailability([FromRoute] int doctorId, [FromQuery] DateTime date)
        {
            try
            {
                var availableSlots = await _availabilityService.GetDoctorAvailability(doctorId, date);
                return StatusCode(StatusCodes.Status200OK, availableSlots);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}