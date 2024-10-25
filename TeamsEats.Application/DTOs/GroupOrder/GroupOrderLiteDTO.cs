using TeamsEats.Domain.Enums;

namespace TeamsEats.Application.DTOs;
public class GroupOrderLiteDTO
{
    public int Id { get; set; }
    public bool IsOwnedByUser { get; set; }
    public bool HasItemInOrder { get; set; }
    public string AuthorName { get; set; }
    public string AuthorPhoto { get; set; }
    public double DeliveryCost { get; set; }
    public string Restaurant { get; set; }
    public GroupOrderStatus Status { get; set; }
    public DateTime? ClosingTime { get; set; }
}
