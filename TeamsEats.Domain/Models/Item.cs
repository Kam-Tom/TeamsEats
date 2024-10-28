using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Domain.Models;

public class Item
{
    public int Id { get; set; }
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Dish { get; set; }
    public double Price { get; set; }
    public string AdditionalInfo { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
}
