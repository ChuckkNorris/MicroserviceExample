using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            services.AddSingleton<IRabbitMqEventBus>(sp => {
                var connection = sp.GetRequiredService<IRabbitMqConnection>();
                return new RabbitMqEventBus(connection);
            });

            return services;
        }

        public static IServiceCollection AddRabbitSubscription<Te, Th>(this IServiceCollection services, string queueName)
            where Te : RabbitMqEvent
            where Th : IRabbitEventHandler<Te>
        {
            services.AddSingleton<IHostedService>(sp => {
                var connection = sp.GetRequiredService<IRabbitMqConnection>();
                Th eventHandler = Activator.CreateInstance<Th>();
                return new RabbitHostedService<Te>(connection, eventHandler, queueName);
            });
            return services;
        }

    }

    public interface IRabbitEventHandler<T> {
        void Handle(T eventBody);
    }

    public class RabbitHostedService<T> : IHostedService {
        private readonly IRabbitMqConnection _connection;
        private readonly IRabbitEventHandler<T> _eventHandler;
        private IModel _channel;
        private readonly string _queueName;

        public RabbitHostedService(IRabbitMqConnection rabbitMqConnection, IRabbitEventHandler<T> eventHandler, string queueName) {
            _connection = rabbitMqConnection;
            _eventHandler = eventHandler;
            _queueName = queueName;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            await _connection.TryConnect();
            if (!_connection.IsConnected) {
                throw new InvalidOperationException("Could not connect to RabbitMQ");
            }
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, // "hello",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                T convertedMessage = JsonConvert.DeserializeObject<T>(message);
                _eventHandler.Handle(convertedMessage);
            };
            _channel.BasicConsume(queue: _queueName, // "hello",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            if (_channel?.IsOpen ?? false)
                _channel.Close();
        }
    }
}
