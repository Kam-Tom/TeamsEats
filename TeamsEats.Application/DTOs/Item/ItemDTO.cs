using DataAnnotationsExtensions;

namespace TeamsEats.Application.DTOs;
public class ItemDTO
{
    public int Id { get; set; }
    public string AuthorPhoto { get; set; }
    public string AuthorName { get; set; }
    public string Dish { get; set; }
    public decimal Price { get; set; }
    public bool IsOwner { get; set; }
    public int OrderId { get; set; }
    public string AdditionalInfo { get; set; }
}
