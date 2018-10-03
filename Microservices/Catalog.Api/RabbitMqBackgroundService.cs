using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabitMqBackgroundService : IHostedService {
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection { get; set; }
    private IModel _channel { get; set; }
    private bool IsConnected { 
        get { return this._connection?.IsOpen ?? false; }
    }

    public RabitMqBackgroundService(string hostname = "localhost", ILogger<RabitMqBackgroundService> logger = null) {
        this._connectionFactory = new ConnectionFactory() { HostName = hostname };
    }
    

    public async Task StartAsync(CancellationToken cancellationToken) {
        while (!this.IsConnected) {
            try {
                this._connection = _connectionFactory.CreateConnection();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            if (!this.IsConnected)
                await Task.Delay(5000);
        }
        this._channel = _connection.CreateModel();
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

public class RabbitMqBackgroundServiceOptions {
    public string Timeout { get; set; }
    public string HostName { get; set; }
}