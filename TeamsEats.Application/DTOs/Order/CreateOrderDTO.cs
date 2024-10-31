using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace TeamsEats.Application.DTOs;

public class CreateOrderDTO
{
    [Length(9, 9, ErrorMessage = "PhoneNumber must be exactly 9 characters long.")]
    public string PhoneNumber { get; set; }

    [Length(26, 26, ErrorMessage = "BankAccount must be exactly 26 characters long.")]
    public string BankAccount { get; set; }

    [Required]
    public string Restaurant { get; set; }

    [Required]
    [Min(0)]
    public decimal MinimalPrice { get; set; }

    [Required]
    [Min(0)]
    public decimal DeliveryFee { get; set; }

    [Required]
    [Min(0)]
    public decimal MinimalPriceForFreeDelivery { get; set; }

    public DateTime? ClosingTime { get; set; }
}
