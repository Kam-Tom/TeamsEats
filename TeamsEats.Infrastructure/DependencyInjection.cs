using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;
using TeamsEats.Infrastructure.Repositories;
using TeamsEats.Infrastructure.Services;

namespace TeamsEats.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("TeamsEats.Infrastructure"));
        });
        services.AddScoped<IItemRepository, ItemsRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IGraphService, GraphService>();

        return services;
    }
}
