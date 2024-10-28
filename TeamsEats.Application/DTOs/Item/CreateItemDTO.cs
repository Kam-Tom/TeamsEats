using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class CreateItemDTO
{

    [Required]
    public int OrderId { get; set; }
    [Required]
    public string Dish { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public double Price { get; set; }

    public string AdditionalInfo { get; set; }

}
