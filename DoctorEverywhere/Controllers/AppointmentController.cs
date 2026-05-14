using System.Security.Claims;
using Azure.Core;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Messaging.DTOs;
using DoctorEverywhere.Messaging.Interfaces;
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
        private readonly IRabbitMqProducerService _producerService;
        private readonly IRabbitMqConsumerService _consumerService;

        public AppointmentController(IAppointmentService appointmentService,
            IRabbitMqProducerService producerService,
            IRabbitMqConsumerService consumerService)
        {
            _appointmentService = appointmentService;
            _producerService = producerService;
            _consumerService = consumerService;
        }

       

        [Authorize(Roles = "Patient")]
        [HttpPost("request")]

            public async Task<IActionResult> CreateAppointment([FromQuery]int doctorId, [FromBody]CreateAppointmentDto dto)
        {
            //POST /appointments 
            //201 Created
            //400, 403 (invalid doctor/time), 409 (double booking) 

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(userId,doctorId,dto);

                var message = new AppointmentMessageDto
                {
                    MessageId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    AppointmentId = createdAppointment.Id,
                    DoctorId = createdAppointment.DoctorId,
                    PatientId = createdAppointment.PatientId,
                    StartingAt = createdAppointment.StartingAt,
                };

                var queueName=$"appointment-{createdAppointment.Id}";
                await _producerService.PublishAsync(message, queueName);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
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
                if(User.IsInRole("Patient"))
                {
                    return StatusCode(StatusCodes.Status200OK, appointments);
                }

                // if(userId is null)
                // {
                //return StatusCode(StatusCodes.Status200OK, new { appointments, notifications = Array.Empty<AppointmentMessageDto>() });
                //return StatusCode(StatusCodes.Status404NotFound, ex.Message);
                // }
                var queueName = $"appointment-{appointments}";
                var result = await _consumerService.ConsumeAsync(queueName);
                return StatusCode(StatusCodes.Status200OK, result);
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
