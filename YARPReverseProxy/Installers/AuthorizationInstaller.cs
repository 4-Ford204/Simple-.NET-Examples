using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace YARPReverseProxy.Installers
{
    public class AuthorizationInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("authorizated",
                    policy => policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser()
                );
            });
        }

        public void ConfigureRequestPiplineAsync(WebApplication app)
        {
            app.UseAuthorization();
        }
    }
}
