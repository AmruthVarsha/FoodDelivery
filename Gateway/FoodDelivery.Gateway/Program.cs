using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace FoodDelivery.Gateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Ocelot configuration
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            builder.Services.AddOcelot(builder.Configuration);

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Use CORS (must be before Ocelot)
            app.UseCors("AllowFrontend");

            // Use Ocelot middleware
            await app.UseOcelot();

            app.Run();
        }
    }
}
