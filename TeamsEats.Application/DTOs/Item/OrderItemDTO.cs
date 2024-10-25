namespace TeamsEats.Application.DTOs;
public class OrderItemDTO
{
    public int Id { get; set; }
    public string AuthorPhoto { get; set; }
    public string AuthorName { get; set; }
    public string DishName { get; set; }
    public double Price { get; set; }
    public bool IsOwner { get; set; }
    public int GroupOrderId { get; set; }
    public string AdditionalInfo { get; set; }
}
