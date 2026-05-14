using DoctorEverywhere.Messaging.DTOs;
using DoctorEverywhere.Messaging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Messaging.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly IRabbitMqProducerService _producer;

        public ProducerController(IRabbitMqProducerService producer)
        {
            _producer = producer;
        }

        //public async Task<IActionResult> Publish([FromBody] AppointmentMessageDto message, [FromRoute] string queueName)
        //{
        // Request

        //var queueName = 
        // await _producer.PublishAsync(message, queueName);
        //return StatusCode(StatusCodes.Status200OK);
        //}

        public async Task<IActionResult> PublishOrder([FromBody]AppointmentMessageDto request)
        {
            request.MessageId = Guid.NewGuid();
            request.CreatedAt = DateTime.UtcNow;
            var queueName = $"appointments.doctor";

            await _producer.PublishAsync(request, queueName);

            return Ok(new
            {
                Message = "Order published successfully",
                request.MessageId
            });
        }
    }
}
