using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("items")]
public class ItemEntity
{
    [Key]
    public int Id { get; set; }
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Dish { get; set; }
    [Precision(10, 2)]
    public decimal Price { get; set; }
    public string AdditionalInfo { get; set; }

    [ForeignKey(nameof(OrderId))]
    public int OrderId { get; set; }
    public virtual OrderEntity Order { get; set; }
}
