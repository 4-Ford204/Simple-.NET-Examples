using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotGateway.Installers
{
    public class OcelotInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            IConfiguration ocelotConfiguration = new ConfigurationBuilder()
                .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddOcelot(ocelotConfiguration);
        }

        public async void ConfigureRequestPiplineAsync(WebApplication app)
        {
            await app.UseOcelot();
        }
    }
}
