using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class CreateOrderItemDTO
{

    [Required]
    public string DishName { get; set; }

    [Required]
    public double Price { get; set; }

    public string AdditionalInfo { get; set; }
    public int GroupOrderId { get; set; }

}
