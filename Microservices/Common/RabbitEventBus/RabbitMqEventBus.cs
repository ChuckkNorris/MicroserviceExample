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
        private readonly IRabbitMqConnection _rabbitMqConnection;

        public RabbitMqEventBus(IRabbitMqConnection rabbitConnection) {
            _rabbitMqConnection = rabbitConnection;
        }

        public async Task Publish(string queueName, RabbitMqEvent rabbitMqEvent) {
            if (!_rabbitMqConnection.IsConnected) {
                await this._rabbitMqConnection.TryConnect();
            }
            using (var channel = _rabbitMqConnection.CreateModel()) {
                channel.QueueDeclare(queue: queueName, // "hello",
                                   durable: false,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);

                // string message = "Hello World!";
                string message = JsonConvert.SerializeObject(rabbitMqEvent);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
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
