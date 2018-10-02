using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabitMqBackgroundService : IHostedService {
    private readonly ConnectionFactory _connectionFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabitMqBackgroundService() {
        this._connectionFactory = new ConnectionFactory() { HostName = "localhost" };
        this._connection = _connectionFactory.CreateConnection();
        this._channel = _connection.CreateModel();
    }
    

    public async Task StartAsync(CancellationToken cancellationToken) {
        _channel.QueueDeclare(queue: "hello",
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) => {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        };
        _channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: consumer);
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        this._connection.Close();
        this._channel.Close();
    }
}