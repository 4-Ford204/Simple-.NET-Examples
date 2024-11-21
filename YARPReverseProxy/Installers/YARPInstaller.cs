namespace YARPReverseProxy.Installers
{
    public class YARPInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddReverseProxy().LoadFromConfig(configuration.GetSection("YARPConfiguration"));
        }

        public void ConfigureRequestPiplineAsync(WebApplication app)
        {
            app.MapReverseProxy();
        }
    }
}
