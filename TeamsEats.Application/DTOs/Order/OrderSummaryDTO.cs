using TeamsEats.Domain.Enums;

namespace TeamsEats.Application.DTOs;
public class OrderSummaryDTO
{
    public int Id { get; set; }
    public bool IsOwner { get; set; }
    public bool IsParticipating { get; set; }
    public string AuthorName { get; set; }
    public string AuthorPhoto { get; set; }
    public double DeliveryCost { get; set; }
    public string Restaurant { get; set; }
    public Status Status { get; set; }
    public DateTime? ClosingTime { get; set; }
}
