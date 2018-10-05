using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Modules.Catalog {
    public class CatalogContext : DbContext {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        
    }

    public class Product {
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
