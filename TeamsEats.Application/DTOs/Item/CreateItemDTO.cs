using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class CreateItemDTO
{

    [Required]
    public int OrderId { get; set; }
    [Required]
    public string Dish { get; set; }

    [Required]
    [Min(0)]
    public decimal Price { get; set; }

    public string AdditionalInfo { get; set; }

}
