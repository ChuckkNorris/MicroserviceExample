using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basket.Api.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// using RabbitEventBus;

namespace Basket.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.ConfigureRabbitMq();
            // services.AddRabbitSubscription<ProductPriceChangedEvent, ProductPriceChangedEventHandler>("ProductPriceChanged");
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    public class ProductPriceChangedEvent : RabbitMqEvent {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductPriceChangedEventHandler : IRabbitEventHandler<ProductPriceChangedEvent> {
        public void Handle(ProductPriceChangedEvent eventBody) {
            Console.WriteLine($"Basket - ProductPriceChangedEvent Handled: {eventBody.ProductName} {eventBody.Price}");
        }
    }

}
