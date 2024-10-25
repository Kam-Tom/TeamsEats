using System.ComponentModel.DataAnnotations;
using TeamsEats.Domain.Enums;

namespace TeamsEats.Domain.Models;

public class GroupOrder
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public string RestaurantName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BankAccount { get; set; }
    public double MinimalPrice { get; set; }
    public double DeliveryFee { get; set; }
    public double MinimalPriceForFreeDelivery { get; set; }
    public GroupOrderStatus Status { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public DateTime? ClosingTime { get; set; }

}
