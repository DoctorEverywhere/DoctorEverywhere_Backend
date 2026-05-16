using System.Security.Claims;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Controllers
{
    [Route("api/[controller]")] //api/patient
    [ApiController]
    public class PatientController : ControllerBase
    {
        private IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]

        public async Task<List<PatientDto>> Get()
        {
            return await _patientService.GetAllPatients();
        }

        [HttpGet("{id}")] //api/patient/1

        public async Task<ActionResult> GetPatientById(int id)
        {   
            try
            {
                var patient = await _patientService.GetPatientById(id);
                return StatusCode(StatusCodes.Status200OK, patient);
            }
            catch(EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my")] //api/patient/my

        public async Task<IActionResult> GetPatientByUserId() {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _patientService.GetPatientByUserId(userId);
                return StatusCode(StatusCodes.Status200OK, patient);
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
