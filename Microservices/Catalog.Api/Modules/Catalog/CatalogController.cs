using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Modules.Catalog;
using Microsoft.AspNetCore.Mvc;
using RabbitEventBus;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        // private readonly CatalogContext _context;
        private readonly IRabbitMqEventBus _eventBus;
        public CatalogController(
            // CatalogContext context,
            IRabbitMqEventBus eventBus
        ) {
            // _context = context;
            _eventBus = eventBus;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            // _context.Products.Add(new Product { Name = "Burger", Price = 12.50m });
            // await _context.SaveChangesAsync();
            await this._eventBus.Publish(queueName: "ProductPriceChanged",rabbitMqEvent: new ProductPriceChangedEvent { Price = 12.62m, ProductName = "Banana" });
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

    public class ProductPriceChangedEvent : RabbitMqEvent {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
