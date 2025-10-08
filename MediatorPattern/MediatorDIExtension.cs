using System.Reflection;
using MediatorPattern.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorPattern;

public static class MediatorDIExtension
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddSingleton<IMediator>(new Mediator(Assembly.GetExecutingAssembly()));
        return services;
    }
}