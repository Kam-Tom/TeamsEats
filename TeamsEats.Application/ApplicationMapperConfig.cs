using AutoMapper;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;
using TeamsEats.Domain.Models;

public class ApplicationMapperConfig
{
    public static void InitializeAutomapper(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Order, OrderSummaryDTO>()
            .ForMember(dest => dest.IsOwner, opt => opt.Ignore())
            .ForMember(dest => dest.MyCost, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorPhoto, opt => opt.Ignore());

        cfg.CreateMap<Order, OrderDetailsDTO>()
            .ForMember(dest => dest.IsOwner, opt => opt.Ignore())
            .ForMember(dest => dest.MyCost, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorPhoto, opt => opt.Ignore());

        cfg.CreateMap<Item, ItemDTO>()
            .ForMember(dest => dest.IsOwner, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorPhoto, opt => opt.Ignore());
        
        cfg.CreateMap<CreateItemDTO, Item>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        cfg.CreateMap<CreateOrderDTO, Order>()
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentDeliveryFee, opt => opt.Ignore());
            
    }
}

