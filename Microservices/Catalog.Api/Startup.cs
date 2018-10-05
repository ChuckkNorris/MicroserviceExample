using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitEventBus;
using Catalog.Api.Modules.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api
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

            var connection = @"Server=catalog.data;Database=master;User=sa;Password=Cobra1234;";

            // This line uses 'UseSqlServer' in the 'options' parameter
            // with the connection string defined above.
            services.AddDbContext<CatalogContext>(
                options => options.UseSqlServer(connection));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            // Creates persistent connection and event bus for RabbitMQ
            services.ConfigureRabbitMq();
            // Subscribe to changes in the "hello" queue
            services.AddRabbitSubscription<UpdateStuffEvent, UpdateStuffEventHandler>("hello");

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

    public class UpdateStuffEvent : RabbitMqEvent {
        public string MyEventMessage { get; set; }
    }

    public class UpdateStuffEventHandler : IRabbitEventHandler<UpdateStuffEvent> {

        public void Handle(UpdateStuffEvent eventBody) {
            Console.WriteLine($"Event Handled: {eventBody}");
        }
    }
}
