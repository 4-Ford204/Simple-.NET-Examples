using Microsoft.AspNetCore.RateLimiting;

namespace YARPReverseProxy.Installers
{
    public class RateLimitingInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
                {
                    options.PermitLimit = 10;
                    options.Window = TimeSpan.FromMinutes(1);
                });
            });
        }

        public void ConfigureRequestPiplineAsync(WebApplication app)
        {
            app.UseRateLimiter();
        }
    }
}
