using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;
using TeamsEats.Infrastructure.Repositories;
using TeamsEats.Infrastructure.Services;
namespace TeamsEats.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseInMemoryDatabase("InMemoryDb"));

        services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
        services.AddScoped<IGroupOrderRepository, GroupOrderRepository>();

        services.AddScoped<IGraphService, GraphService>();

        return services;
    }

}
