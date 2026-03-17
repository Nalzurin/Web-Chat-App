using System;
using System.Threading.Tasks;
using back_end.Features.Keys.Dto;

namespace back_end.Features.Keys.Interfaces;

public interface IKeyRepository
{
    Task UploadKeyBundleAsync(Guid userId, KeyUploadDto dto);
    Task<KeyBundleResponseDto?> GetAndConsumeOneTimePreKeyAsync(Guid userId);
    Task<bool> DeleteDeviceAsync(Guid userId, string deviceId);
}
