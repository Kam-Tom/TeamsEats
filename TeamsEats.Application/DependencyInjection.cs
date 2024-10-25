using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TeamsEats.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(m => m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }

}
