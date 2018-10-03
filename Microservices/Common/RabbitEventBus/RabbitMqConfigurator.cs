using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitEventBus
{
    public static class RabbitMqConfigurator
    {
        public static IServiceCollection ConfigureRabbitMq(this IServiceCollection services) {
            services.AddSingleton<IRabbitMqConnection>(sp => {
                var logger = sp.GetRequiredService<ILogger<RabbitMqConnection>>();
                return new RabbitMqConnection("rabbitmq", logger);
            });

            return services;
        }

        public static IServiceCollection AddRabbitSubscription<T>(this IServiceCollection services, string queueName)
            where T : IRabbitEventHandler    
        {
            services.AddSingleton<IHostedService>(sp => {
                var connection = sp.GetRequiredService<IRabbitMqConnection>();
                T eventHandler = Activator.CreateInstance<T>();
                return new RabbitHostedService(connection, eventHandler);
                // return new RabbitMqConnection("rabbitmq", logger);
            });
            return services;
        }

    }

    public interface IRabbitEventHandler {
        string QueueName { get; }
        void Handle(string eventBody);

    }

    public class RabbitHostedService : IHostedService {
        private readonly IRabbitMqConnection _connection;
        private readonly IRabbitEventHandler _eventHandler;
        private IModel _channel;

        public RabbitHostedService(IRabbitMqConnection rabbitMqConnection, IRabbitEventHandler eventHandler) {
            _connection = rabbitMqConnection;
            _eventHandler = eventHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _connection.TryConnect();
            if (!_connection.IsConnected) {
                throw new InvalidOperationException("Could not connect to RabbitMQ");
            }
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _eventHandler.QueueName, // "hello",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                _eventHandler.Handle(message);
            };
            _channel.BasicConsume(queue: _eventHandler.QueueName, // "hello",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            _channel.Close();
        }
    }
}
