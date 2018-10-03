using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitEventBus
{

  

    public class RabbitMqEventBus
    {
        private readonly RabbitMqConnection _rabbitMqConnection;

        public RabbitMqEventBus(RabbitMqConnection rabbitConnection) {
            _rabbitMqConnection = rabbitConnection;
        }

        public async Task Publish(IRabbitMqEvent rabbitMqEvent) {
            if (!_rabbitMqConnection.IsConnected) {
                await this._rabbitMqConnection.TryConnect();
            }
            //using (var channel = _rabbitMqConnection.CreateModel()) {
            //    channel.ExchangeDec
            //}
            
        }

        public static void Subscribe<Ti, To>() {

        }
    }

    public class RabbitMqEvent {
        public RabbitMqEvent() {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }

    }

    public interface IRabbitMqEvent {
        id
    }
}
