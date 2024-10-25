using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Domain.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public string DishName { get; set; }
    public double Price { get; set; }
    public string AdditionalInfo { get; set; }
    public int GroupOrderId { get; set; }
    public GroupOrder GroupOrder { get; set; }
}
