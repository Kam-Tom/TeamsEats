using DataAnnotationsExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamsEats.Domain.Enums;

[Table("orders")]
public class OrderEntity
{
    [Key]
    public int Id { get; set; }
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Restaurant { get; set; }
    [Length(9, 9)]
    public string PhoneNumber { get; set; }
    [Length(26, 26)]
    public string BankAccount { get; set; }
    [Min(0)]
    [Precision(10, 2)]
    public decimal MinimalPrice { get; set; }
    [Min(0)]
    [Precision(10, 2)]
    public decimal CurrentDeliveryFee { get; set; }
    [Min(0)]
    [Precision(10, 2)]
    public decimal CurrentPrice { get; set; }
    [Min(0)]
    [Precision(10, 2)]
    public decimal DeliveryFee { get; set; }
    [Min(0)]
    [Precision(10, 2)]
    public decimal MinimalPriceForFreeDelivery { get; set; }
    public Status Status { get; set; }
    public virtual List<ItemEntity> Items { get; set; }
    public DateTime? ClosingTime { get; set; }

}
