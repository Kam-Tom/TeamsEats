using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class UpdateItemDTO
{
    [Required]
    public int OrderId { get; set; }
    public string Dish { get; set; }

    [Min(0)]
    public decimal Price { get; set; }
    
    public string AdditionalInfo { get; set; }
    

}
