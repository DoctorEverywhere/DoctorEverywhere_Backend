using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorEverywhere.Controllers
{
    [Route("api/[controller]")] //api/doctor
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Doctor, Patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetbyId([FromRoute] int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorById(id);
                return StatusCode(StatusCodes.Status200OK, doctor);
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

        [Authorize(Roles = "Patient")]
        [HttpGet("search")]
        public async Task<ActionResult> GetDoctorBySpecialty([FromQuery] int? specialty)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorBySpecialty(specialty);
                return StatusCode(StatusCodes.Status200OK, doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("me")]
        public async Task<ActionResult> GetMyProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var doctor = await _doctorService.GetMyProfile(userId);
                return StatusCode(StatusCodes.Status200OK, doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteDoctor()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _doctorService.DeleteDoctorAsync(userId);
                return StatusCode(StatusCodes.Status200OK, "Doctor deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}