using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;

public class CreateOrderDTO
{
    [Length(9, 11)]
    public string? PhoneNumber { get; set; }

    [Length(8, 17)]
    public string? BankAccount { get; set; }

    [Required]
    public string Restaurant { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double MinimalPrice { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double DeliveryFee { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double MinimalPriceForFreeDelivery { get; set; }

    public DateTime? ClosingTime { get; set; }
}
