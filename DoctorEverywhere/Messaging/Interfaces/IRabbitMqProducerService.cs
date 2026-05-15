using DoctorEverywhere.Messaging.DTOs;

namespace DoctorEverywhere.Messaging.Interfaces
{
    public interface IRabbitMqProducerService
    {
        public Task PublishAsync(AppointmentMessageDto message, string queueName);
    }
}
