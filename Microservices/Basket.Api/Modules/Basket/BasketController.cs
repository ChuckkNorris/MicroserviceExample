using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitEventBus;
using RabbitMQ.Client;

namespace Basket.Api.Controllers
{
    [Route("api/[controller]")]
    public class BasketController : Controller
    {
        private readonly IRabbitMqEventBus _eventBus;
        public BasketController(
            IRabbitMqEventBus eventBus
        ) {
            _eventBus = eventBus;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var message = new UpdateStuffEvent() {
                MyEventMessage = "Yo yo yo, what up party people?"
            };
            _eventBus.Publish("hello", message);
            //var factory = new ConnectionFactory() { 
            //    HostName = "rabbitmq",
            //    Port = 5672,
            //    UserName = "user",
            //    Password = "pass",
            //    VirtualHost = "/",
            //    AutomaticRecoveryEnabled = true,
            //    NetworkRecoveryInterval = TimeSpan.FromSeconds(15)
            //};
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel()) {
            //    channel.QueueDeclare(queue: "hello",
            //                         durable: false,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    string message = "Hello World!";
            //    var body = Encoding.UTF8.GetBytes(message);

            //    channel.BasicPublish(exchange: "",
            //                         routingKey: "hello",
            //                         basicProperties: null,
            //                         body: body);
            //    Console.WriteLine(" [x] Sent {0}", message);
            //}
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class UpdateStuffEvent : RabbitMqEvent {
        public string MyEventMessage { get; set; }
    }
}
