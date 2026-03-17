using System;

namespace back_end.Features.Keys.Dto;

public record KeyUploadDto(string DeviceId, int RegistrationId, string IdentityKey, string SignedPreKey, string SignedPreKeySig, string[] PreKeys);
