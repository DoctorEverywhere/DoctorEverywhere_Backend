using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetbyId(int id)
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
    }
}