using System;

namespace back_end.Models;

public enum MessageStatus : short
{
    Sent = 0,
    Delivered = 1,
    Read = 2
}

public class Message
{
    public long Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string CipherText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
}
