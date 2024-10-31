using AutoMapper;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure;

public class InfrastructureMapperConfig
{
    public static void InitializeAutomapper(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<OrderEntity, Order>();
        cfg.CreateMap<Order, OrderEntity>();
        cfg.CreateMap<ItemEntity, Item>();
        cfg.CreateMap<Item, ItemEntity>();
    }

}
