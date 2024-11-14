using Authentication.Repositories.Implementations.SLTs;
using Authentication.Repositories.Interfaces.SLTs;

namespace Authentication.Installers
{
    public class SystemInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.AllowTrailingCommas = false;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddTransient<ITransient, Transient>();
            services.AddScoped<IScoped, Scoped>();
            services.AddSingleton<ISingleton, Singleton>();
        }

        public void ConfigureRequestPiplineAsync(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
