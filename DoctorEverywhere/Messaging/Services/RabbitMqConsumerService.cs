using System.Text;
using System.Text.Json;
using DoctorEverywhere.Messaging.Configuration;
using DoctorEverywhere.Messaging.DTOs;
using DoctorEverywhere.Messaging.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DoctorEverywhere.Messaging.Services
{
    public sealed class RabbitMqConsumerService : IRabbitMqConsumerService
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMqConsumerService(IOptions<RabbitMqSettings> options)
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

            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();


        }

        public async Task<AppointmentMessageDto?> ConsumeAsync(string queueName)
        {

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var result = await _channel.BasicGetAsync(
                queue: queueName, 
                autoAck: false
            );

            if (result is null)
            {
                return null;
            }

            try
            {
                var body = result.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                // Serialization means to convert an object into that string, and deserialization is its inverse operation (convert string -> object)
                var message = JsonSerializer.Deserialize<AppointmentMessageDto>(json);

                await _channel.BasicAckAsync(
                    deliveryTag: result.DeliveryTag,
                    multiple: false
                );

                return message;
            }
            catch
            {

                await _channel.BasicNackAsync(
                    deliveryTag: result.DeliveryTag,
                    multiple: false,
                    requeue: true
                );

                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();

            _connection?.Dispose();
        }


    }

}
