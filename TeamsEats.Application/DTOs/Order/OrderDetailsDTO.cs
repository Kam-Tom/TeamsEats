using TeamsEats.Domain.Enums;
namespace TeamsEats.Application.DTOs;

public class OrderDetailsDTO
{
    public int Id { get; set; }
    public bool IsOwner { get; set; }
    public string AuthorName { get; set; }
    public string AuthorPhoto { get; set; }
    public string Restaurant { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BankAccount { get; set; }
    public double MinimalPrice { get; set; }
    public double DeliveryCost { get; set; }
    public double MinimalPriceForFreeDelivery { get; set; }
    public List<ItemDTO> Items { get; set; }
    public Status Status { get; set; }
    public DateTime? ClosingTime { get; set; }
}