using System.Security.Claims;
using Azure.Core;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
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

        public async Task<IActionResult> CreateAppointment([FromQuery] int doctorId, [FromBody] CreateAppointmentDto dto)
        {

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(userId, doctorId, dto);

                var message = new AppointmentMessageDto
                {
                    MessageId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    AppointmentId = createdAppointment.Id,
                    DoctorId = createdAppointment.DoctorId,
                    PatientId = createdAppointment.PatientId,
                    StartingAt = createdAppointment.StartingAt,
                };

                var queueName = $"appointment-doctor-{createdAppointment.DoctorId}";
                await _producerService.PublishAsync(message, queueName);
                return StatusCode(StatusCodes.Status201Created);
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


        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var appointments = await _appointmentService.GetUserAppointments(userId);

                if (User.IsInRole("Patient"))
                {
                    return StatusCode(StatusCodes.Status200OK, appointments);
                }
                //Otherwise userId is doctor,so we need to consume messages from RabbitMQ

                //grab doctorId from service so the queue names match
                var doctorId = await _appointmentService.GetDoctorIdForUser(userId);

                if (doctorId == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Doctor profile not found");
                }

                var queueName = $"appointment-doctor-{doctorId}";

                //Consume messages
                var result = await _consumerService.ConsumeAsync(queueName);

                //return all appointments for doctor,including notifications about new ones(if there no new messages to consume,null will be returned instead)
                return StatusCode(StatusCodes.Status200OK, new { appointments, result });
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

        [Authorize(Roles = "Doctor,Patient")]

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus([FromRoute(Name = "id")] int appointmentId, [FromBody] UpdateAppointmentStatusDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (User.IsInRole("Patient"))
                {
                    if (dto.StatusId != AppointmentStatus.Cancelled) //Patient is only allowed to cancel
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to perform this operation");
                    }

                    var updatedAppointment = await _appointmentService.UpdateAppointmentStatus(userId, appointmentId, dto);

                    if (updatedAppointment == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, "Appointment not found");
                    }
                    return StatusCode(StatusCodes.Status200OK, updatedAppointment);

                }
                else //You are a doctor,so you can do anything except cancel
                {
                    if (dto.StatusId == AppointmentStatus.Cancelled)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to cancel an appointment");
                    }
                    var updatedAppointment = await _appointmentService.UpdateAppointmentStatus(userId, appointmentId, dto);

                    if (updatedAppointment == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, "Appointment not found");
                    }
                    return StatusCode(StatusCodes.Status200OK, updatedAppointment);
                }
                    
                   
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
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

        /*
        public async Task<IActionResult> UpdateAppointmentStatus([FromRoute(Name = "id")] int appointmentId, [FromBody] UpdateAppointmentStatusDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var updatedAppointment = await _appointmentService.UpdateAppointmentStatus(userId, appointmentId, dto);
                if (updatedAppointment == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Appointment not found");
                }
                    return StatusCode(StatusCodes.Status200OK, updatedAppointment);

            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
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
        */
    }
}
