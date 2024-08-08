using MasstransitRabbitMQ.Producer.API.DIs.Extensions;

namespace MasstransitRabbitMQ.Producer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Container Services
            builder.Services.AddControllers();

            // Swagger/OpenAPI Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Masstransit RabbitMQ Configuration
            builder.Services.AddMasstransitRabbitMQConfiguration(builder.Configuration);

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
