using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleCut.Common.Options;

namespace SimpleCut.Infrastructure.Dependency
{
    public static class OptionsModule
    {
        public static IServiceCollection AddOptionsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenOptions>(configuration.GetSection(TokenOptions.Position));

            return services;
        }
    }
}
