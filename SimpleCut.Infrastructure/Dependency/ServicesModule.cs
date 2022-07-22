using Microsoft.Extensions.DependencyInjection;
using SimpleCut.Services.Accounts;

namespace SimpleCut.Infrastructure.Dependency
{
    public static class ServicesModule
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
