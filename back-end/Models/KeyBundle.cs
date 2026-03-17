using System;
using System.Collections.Generic;

namespace back_end.Models;

public class KeyBundle
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public int RegistrationId { get; set; }
    public string IdentityKey { get; set; } = string.Empty;
    public string SignedPreKey { get; set; } = string.Empty;
    public string SignedPreKeySig { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OneTimePreKey> OneTimePreKeys { get; set; } = new List<OneTimePreKey>();
}
