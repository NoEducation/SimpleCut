using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SimpleCut.Infrastructure.BehaviourPipelines;
using SimpleCut.Infrastructure.Cqrs;
using System.Reflection;

namespace SimpleCut.Infrastructure.Dependency
{
    public static class CqrsModule
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services)
        {
            services.AddMediatR( Assembly.Load("SimpleCut.Logic"));
            services.AddTransient<IDispatcher, Dispatcher>();
            services.AddValidatorsFromAssembly(Assembly.Load("SimpleCut.Logic"));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
