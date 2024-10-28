using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class UpdateItemDTO
{
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    public string Dish { get; set; }

    public double Price { get; set; }
    
    public string AdditionalInfo { get; set; }
    

}
