﻿using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitEventBus
{
    public interface IRabbitMqConnection {
        bool IsConnected { get; }
        Task<bool> TryConnect();

        IModel CreateModel();
    }

    public class RabbitMqConnection : IRabbitMqConnection {

        private readonly ILogger<RabbitMqConnection> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection { get; set; }
        private IModel _channel { get; set; }
       

        private readonly int _maxRetries;

        public RabbitMqConnection(
            string hostname = "localhost",
            ILogger<RabbitMqConnection> logger = null,
            int maxRetries = 5
        ) {
            this._connectionFactory = new ConnectionFactory() {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "user",
                Password = "pass",
                VirtualHost = "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(15)
            };
            _maxRetries = maxRetries;
        }

        public bool IsConnected {
            get { return this._connection?.IsOpen ?? false; }
        }

        public IModel CreateModel() {
            if (!IsConnected) {
                throw new InvalidOperationException("No RabbitMQ connections available...");
            }
            return _connection.CreateModel();
            
        }

        public async Task<bool> TryConnect() {
            int currentRetry = 0;
            
            while (!this.IsConnected && currentRetry < _maxRetries) {
                try {
                    this._connection = _connectionFactory.CreateConnection();
                }
                catch (Exception ex) {
                    Console.WriteLine($"{_connectionFactory?.HostName}:{_connectionFactory?.Port} -> {ex.Message}");
                }
                currentRetry++;
                if (!this.IsConnected)
                    await Task.Delay(5000);
            }
            return this.IsConnected;
        }


        //public async Task StartAsync(CancellationToken cancellationToken) {
        //    while (!this.IsConnected) {
        //        try {
        //            this._connection = _connectionFactory.CreateConnection();
        //        }
        //        catch (Exception ex) {
        //            Console.WriteLine($"{_connectionFactory?.HostName}:{_connectionFactory?.Port} -> {ex.Message}");
        //        }
        //        if (!this.IsConnected)
        //            await Task.Delay(5000);
        //    }
        //    this._channel = _connection.CreateModel();
        //    _channel.QueueDeclare(queue: "hello",
        //                          durable: false,
        //                          exclusive: false,
        //                          autoDelete: false,
        //                          arguments: null);

        //    var consumer = new EventingBasicConsumer(_channel);
        //    consumer.Received += (model, ea) => {
        //        var body = ea.Body;
        //        var message = Encoding.UTF8.GetString(body);
        //        Console.WriteLine(" [x] Received {0}", message);
        //    };
        //    _channel.BasicConsume(queue: "hello",
        //                         autoAck: true,
        //                         consumer: consumer);
        //}

        //public async Task StopAsync(CancellationToken cancellationToken) {
        //    this._connection.Close();
        //    this._channel.Close();
        //}
    }
}
