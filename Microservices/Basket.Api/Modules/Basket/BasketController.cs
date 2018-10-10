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
        // private readonly IRabbitMqEventBus _eventBus;
        public BasketController(
            // IRabbitMqEventBus eventBus
        ) {
            // _eventBus = eventBus;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> GetUserBasket()
        {
            // var message = new UpdateStuffEvent() {
            //     MyEventMessage = "Yo yo yo, what up party people?"
            // };
            // _eventBus.Publish("hello", message);
            return new string[] { "basket", "value2" };
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
