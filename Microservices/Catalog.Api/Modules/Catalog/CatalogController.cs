using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Modules.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _context;
        public CatalogController(
            CatalogContext context    
        ) {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            _context.Products.Add(new Product { Name = "Burger", Price = 12.50m });
            await _context.SaveChangesAsync();
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
}
