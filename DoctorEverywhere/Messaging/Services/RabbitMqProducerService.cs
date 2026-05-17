using System.Text;
using System.Text.Json;
using DoctorEverywhere.Messaging.Configuration;
using DoctorEverywhere.Messaging.DTOs;
using DoctorEverywhere.Messaging.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DoctorEverywhere.Messaging.Services
{
    public sealed class RabbitMqProducerService : IRabbitMqProducerService, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;

        public RabbitMqProducerService(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public async Task PublishAsync(AppointmentMessageDto message,string queueName)
        {
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory:false,
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
