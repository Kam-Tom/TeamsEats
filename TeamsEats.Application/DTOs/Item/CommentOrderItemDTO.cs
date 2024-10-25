using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class CommentOrderItemDTO
{
    [Required]
    public int OrderItemID { get; set; }
    [Required]
    public string Message { get; set; }
}
