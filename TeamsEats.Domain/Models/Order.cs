using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;
using TeamsEats.Domain.Enums;

namespace TeamsEats.Domain.Models;

public class Order
{
    public int Id { get; set; }
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Restaurant { get; set; }
    public string PhoneNumber { get; set; }
    public string BankAccount { get; set; }
    public decimal MinimalPrice { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal CurrentDeliveryFee { get; set; }
    public decimal MinimalPriceForFreeDelivery { get; set; }
    public decimal CurrentPrice { get; set; }
    public Status Status { get; set; }
    public List<Item> Items { get; set; }
    public DateTime? ClosingTime { get; set; }

}
