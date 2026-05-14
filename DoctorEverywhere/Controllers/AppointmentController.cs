using System.Security.Claims;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

//POST /appointments 
//201 Created
//400, 403 (invalid doctor/time), 409 (double booking) 
//PATCH /appointments/{id} 
//200 OK
//400, 403 (wrong role), 404, 409 (invalid state transition) 

namespace DoctorEverywhere.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

       

        [Authorize(Roles = "Patient")]
        [HttpPost("request")]

            public async Task<IActionResult> CreateAppointment(int doctorId, CreateAppointmentDto dto)
        {
            //POST /appointments 
            //201 Created
            //400, 403 (invalid doctor/time), 409 (double booking) 

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _appointmentService.CreateAppointmentAsync(userId,doctorId,dto);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }

            
        }

        
        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointments()
        {
           try
           {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var appointments = await _appointmentService.GetUserAppointments(userId);
                return StatusCode(StatusCodes.Status200OK, appointments);
            }
            catch(EntityNotFoundException ex)
            {
               return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
               return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        

        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var appointment = await _appointmentService.GetAppointmentById(userId, id);
                return StatusCode(StatusCodes.Status200OK, appointment);
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
