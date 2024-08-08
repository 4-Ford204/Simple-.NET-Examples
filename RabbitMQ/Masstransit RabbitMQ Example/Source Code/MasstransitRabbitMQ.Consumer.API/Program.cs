using MasstransitRabbitMQ.Consumer.API.DIs.Extensions;
using Serilog;

namespace MasstransitRabbitMQ.Consumer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Container Services
            builder.Services.AddControllers();

            // Serilog Configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(builder.Configuration)
                .CreateLogger();

            builder.Logging.ClearProviders().AddSerilog();

            builder.Host.UseSerilog();

            // Swagger/OpenAPI Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Masstransit RabbitMQ Configuration
            builder.Services.AddMasstransitRabbitMQConfiguration(builder.Configuration);

            // MediatR Configuration
            builder.Services.AddMediaRConfiguration();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
