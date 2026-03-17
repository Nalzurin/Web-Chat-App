using System;

namespace back_end.Features.Keys.Dto;

public record KeyBundleResponseDto(string DeviceId, int RegistrationId, string IdentityKey, string SignedPreKey, string SignedPreKeySig, string? OneTimePreKey);
