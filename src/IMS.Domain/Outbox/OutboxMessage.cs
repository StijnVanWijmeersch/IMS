using IMS.SharedKernel;

namespace IMS.Domain.Outbox;
public sealed class OutboxMessage : Entity
{
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }

    private OutboxMessage() { }

    public static OutboxMessage Create(string type, string content)
    {
        var outboxMessage = new OutboxMessage
        {
            Type = type,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        return outboxMessage;
    }
}
