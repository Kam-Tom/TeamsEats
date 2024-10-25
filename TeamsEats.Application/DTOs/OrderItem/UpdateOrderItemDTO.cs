using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class UpdateOrderItemDTO
{

    [Required]
    public string DishName { get; set; }

    [Required]
    public double Price { get; set; }

    public string AdditionalInfo { get; set; }
    public int OrderItemID { get; set; }
    public int GroupOrderId { get; set; }

}
