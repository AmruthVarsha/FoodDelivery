using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace AdminService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
            builder.Services.AddOcelot();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerForOcelot(builder.Configuration);
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Food Delivery Gateway API",
                    Version = "v1",
                    Description = "API Gateway for Food Delivery Platform"
                });
            });

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowFrontEnd",policy => 
                    policy.WithOrigins("http://localhost:4200", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });

            var app = builder.Build();

            app.UseCors("AllowFrontEnd");
            
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });
            
            await app.UseOcelot();

            app.Run();
        }
    }
}
