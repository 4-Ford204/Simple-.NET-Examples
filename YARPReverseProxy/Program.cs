using YARPReverseProxy.Installers;

namespace YARPReverseProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.InstallServicesInAssembly(builder.Configuration);

            var app = builder.Build();

            app.ConfigureRequestPiplineInAssembly();

            app.Run();
        }
    }
}
