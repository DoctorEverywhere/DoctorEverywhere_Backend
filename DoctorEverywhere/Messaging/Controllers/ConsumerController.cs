/*
using System.Net;
using DoctorEverywhere.Domain;
using DoctorEverywhere.Exceptions;
using Microsoft.AspNetCore.Mvc;
using DoctorEverywhere.DTOs;
using Microsoft.AspNetCore.Http;
using DoctorEverywhere.Messaging.Interfaces;

namespace DoctorEverywhere.Messaging.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerController : ControllerBase
    {
        private readonly IRabbitMqConsumerService _consumer;    

        public ConsumerController(IRabbitMqConsumerService consumer)
        {
            _consumer = consumer;
        }

        [HttpGet]
        public async Task<IActionResult> Consume([FromRoute] string id)
        {
            try
            {
                var result = await _consumer.ConsumeAsync(id);
                return StatusCode(StatusCodes.Status200OK, result);
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

*/