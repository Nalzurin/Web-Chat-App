using System;

namespace back_end.Models;

public enum FriendshipStatus : short
{
    Pending = 0,
    Accepted = 1,
    Blocked = 2
}

public class Friendship
{
    public Guid Id { get; set; }
    public Guid RequesterId { get; set; }
    public Guid AddresseeId { get; set; }
    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
