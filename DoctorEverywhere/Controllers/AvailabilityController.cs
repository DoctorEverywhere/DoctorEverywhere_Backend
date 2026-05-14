using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Services.Interfaces;
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
    }
}