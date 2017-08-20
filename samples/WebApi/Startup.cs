using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recom.Bus.RabbitMQ;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRecomRabbitMQ(Configuration);
            services.AddSingleton<TestSubscription>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            appLifetime.ApplicationStarted.Register(() =>
            {
                var bus = app.ApplicationServices.GetService<IBus>();
                bus.CreateExchange("TestExchange");

                var subscriber = new AutoSubscriber(bus, app.ApplicationServices);
                subscriber.Subscribe(Assembly.GetExecutingAssembly());
            });

            appLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetService<IBus>().Dispose();
            });
        }
    }
}
