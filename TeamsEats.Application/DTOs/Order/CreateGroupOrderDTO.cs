using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class CreateGroupOrderDTO
{
    public string? PhoneNumber { get; set; }
    public string? BankAccount { get; set; }
    [Required]
    public string RestaurantName { get; set; }
    [Required]
    public double MinimalPrice { get; set; }
    [Required]
    public double DeliveryFee { get; set; }
    [Required]
    public double MinimalPriceForFreeDelivery { get; set; }
    public DateTime? ClosingTime { get; set; }
}
