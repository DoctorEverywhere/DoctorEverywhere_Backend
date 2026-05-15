using DoctorEverywhere.Messaging.DTOs;

namespace DoctorEverywhere.Messaging.Interfaces
{
    public interface IRabbitMqConsumerService
    {
        public Task<AppointmentMessageDto?> ConsumeAsync(string queueName);
    }
}
