using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            options.UseLazyLoadingProxies()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                    x => x.MigrationsAssembly("TeamsEats.Infrastructure"));
        });

        services.AddScoped<IOrderRepository, OrderRepository>();


        services.AddAutoMapper(cfg => InfrastructureMapperConfig.InitializeAutomapper(cfg));
        services.AddScoped<IGraphService, GraphService>();

        return services;
    }
}
