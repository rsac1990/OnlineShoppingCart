namespace OnlineShopping.Data.Catalog.Outbox;

using Domain.Catalog.Entities;

public class OutboxMessage : Entity
{
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}