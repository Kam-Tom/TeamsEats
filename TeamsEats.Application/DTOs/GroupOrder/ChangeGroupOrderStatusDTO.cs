using TeamsEats.Domain.Enums;

namespace TeamsEats.Application.DTOs;
public class ChangeGroupOrderStatusDTO
{
    public GroupOrderStatus Status { get; set; }
    public int GroupOrderID { get; set; }

}
