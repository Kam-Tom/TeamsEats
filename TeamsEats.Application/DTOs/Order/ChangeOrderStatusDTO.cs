using System.ComponentModel.DataAnnotations;
using TeamsEats.Domain.Enums;

namespace TeamsEats.Application.DTOs;
public class ChangeOrderStatusDTO
{
    public int Id { get; set; }
    [Required]
    public Status Status { get; set; }

}
