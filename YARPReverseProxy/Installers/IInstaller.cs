namespace YARPReverseProxy.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
        void ConfigureRequestPiplineAsync(WebApplication app);
    }
}
