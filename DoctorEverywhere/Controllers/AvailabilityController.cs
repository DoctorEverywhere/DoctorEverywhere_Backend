using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Services;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("slots")]
        public async Task<IActionResult> CreateorUpdateAvailability([FromBody] List<AvailabilityDto> availabilityDtos)
        {

            try
            {
                var userId = "d95eee14-6340-4840-95c2-db12554843e5"; //User.FindFirstValue(ClaimTypes.NameIdentifier); //to-do after authentication is implemented
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