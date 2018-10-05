using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitEventBus
{
    public class RabbitMqEventBus : IRabbitMqEventBus {

        internal const string EXAMPLE_EXCHANGE_NAME = "microservice_example_event_bus";
        private readonly IRabbitMqConnection _rabbitMqConnection;

        public RabbitMqEventBus(IRabbitMqConnection rabbitConnection) {
            _rabbitMqConnection = rabbitConnection;
        }

        public async Task Publish(string queueName, RabbitMqEvent rabbitMqEvent) {
            if (!_rabbitMqConnection.IsConnected) {
                await this._rabbitMqConnection.TryConnect();
            }

            string message = JsonConvert.SerializeObject(rabbitMqEvent);
            var body = Encoding.UTF8.GetBytes(message);

            using (var channel = _rabbitMqConnection.CreateModel()) {
                string eventName = rabbitMqEvent.GetType().Name;
                channel.ExchangeDeclare(exchange: EXAMPLE_EXCHANGE_NAME, type: "direct");
                var channelProps = channel.CreateBasicProperties();
                channelProps.DeliveryMode = 2; // Persistent (stored to disk)
                channel.BasicPublish(exchange: EXAMPLE_EXCHANGE_NAME,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: channelProps,
                    body: body);

                //channel.QueueDeclare(queue: queueName,
                //                   durable: false,
                //                   exclusive: false,
                //                   autoDelete: false,
                //                   arguments: null);

                //string message = JsonConvert.SerializeObject(rabbitMqEvent);
                //var body = Encoding.UTF8.GetBytes(message);

                //channel.BasicPublish(exchange: "",
                //                     routingKey: queueName,
                //                     basicProperties: null,
                //                     body: body);
                Console.WriteLine($" [x] Sent {message}, {eventName}, {queueName}");
            }

        }

        public static void Subscribe<Ti, To>() {
            throw new NotImplementedException();
        }
    }

    public interface IRabbitMqEventBus {
        Task Publish(string queueName, RabbitMqEvent rabbitMqEvent);
    }

    public class RabbitMqEvent {
        public RabbitMqEvent() {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }

    }

}
