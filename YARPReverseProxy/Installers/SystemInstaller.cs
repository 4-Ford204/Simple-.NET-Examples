namespace YARPReverseProxy.Installers
{
    public class SystemInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void ConfigureRequestPiplineAsync(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();
        }
    }
}
