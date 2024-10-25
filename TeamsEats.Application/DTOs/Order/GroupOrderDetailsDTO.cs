using TeamsEats.Domain.Enums;
namespace TeamsEats.Application.DTOs;

public class GroupOrderDetailsDTO
{
    public int Id { get; set; }
    public bool isOwnedByUser { get; set; }
    public string AuthorName { get; set; }
    public string AuthorPhoto { get; set; }
    public string PhoneNumber { get; set; }
    public string Restaurant { get; set; }
    public string BankAccount { get; set; }
    public double MinimalPrice { get; set; }
    public double DeliveryCost { get; set; }
    public double MinimalPriceForFreeDelivery { get; set; }
    public List<OrderItemDTO> OrderItems { get; set; }
    public GroupOrderStatus Status { get; set; }
    public DateTime? ClosingTime { get; set; }
}