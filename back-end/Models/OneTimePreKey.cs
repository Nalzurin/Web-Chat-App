using System;

namespace back_end.Models;

public class OneTimePreKey
{
    public Guid Id { get; set; }
    public Guid KeyBundleId { get; set; }
    public string PreKey { get; set; } = string.Empty;
}
