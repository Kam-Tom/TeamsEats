using System.ComponentModel.DataAnnotations;

namespace TeamsEats.Application.DTOs;
public class CommentItemDTO
{
    public int ItemId { get; set; }
    [Required]
    public string Message { get; set; }
}
