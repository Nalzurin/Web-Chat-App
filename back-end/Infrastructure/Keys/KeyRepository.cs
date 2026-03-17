using System;
using System.Linq;
using System.Threading.Tasks;
using back_end.Data;
using back_end.Features.Keys.Dto;
using back_end.Features.Keys.Interfaces;
using back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace back_end.Infrastructure.Keys;

public class KeyRepository : IKeyRepository
{
    private readonly ChatDbContext _db;

    public KeyRepository(ChatDbContext db)
    {
        _db = db;
    }

    public async Task UploadKeyBundleAsync(Guid userId, KeyUploadDto dto)
    {
        // Replace existing bundle for (userId, deviceId)
        var existing = await _db.KeyBundles!
            .Include(k => k.OneTimePreKeys)
            .FirstOrDefaultAsync(k => k.UserId == userId && k.DeviceId == dto.DeviceId);

        if (existing != null)
        {
            // remove old one-time prekeys
            _db.OneTimePreKeys!.RemoveRange(existing.OneTimePreKeys);
            existing.RegistrationId = dto.RegistrationId;
            existing.IdentityKey = dto.IdentityKey;
            existing.SignedPreKey = dto.SignedPreKey;
            existing.SignedPreKeySig = dto.SignedPreKeySig;
            existing.CreatedAt = DateTime.UtcNow;
            existing.OneTimePreKeys = dto.PreKeys.Select(pk => new OneTimePreKey { KeyBundleId = existing.Id, PreKey = pk }).ToList();
            await _db.SaveChangesAsync();
            return;
        }

        var bundle = new KeyBundle
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeviceId = dto.DeviceId,
            RegistrationId = dto.RegistrationId,
            IdentityKey = dto.IdentityKey,
            SignedPreKey = dto.SignedPreKey,
            SignedPreKeySig = dto.SignedPreKeySig,
            CreatedAt = DateTime.UtcNow
        };

        bundle.OneTimePreKeys = dto.PreKeys.Select(pk => new OneTimePreKey { KeyBundleId = bundle.Id, PreKey = pk }).ToList();

        await _db.KeyBundles!.AddAsync(bundle);
        await _db.SaveChangesAsync();
    }

    public async Task<KeyBundleResponseDto?> GetAndConsumeOneTimePreKeyAsync(Guid userId)
    {
        // return a bundle for any device (prefer first) and atomically remove one one-time prekey if available
        var bundle = await _db.KeyBundles!
            .Include(k => k.OneTimePreKeys)
            .Where(k => k.UserId == userId)
            .OrderBy(k => k.CreatedAt)
            .FirstOrDefaultAsync();

        if (bundle == null)
            return null;

        string? oneTime = null;
        var otp = bundle.OneTimePreKeys.FirstOrDefault();
        if (otp != null)
        {
            oneTime = otp.PreKey;
            _db.OneTimePreKeys!.Remove(otp);
            await _db.SaveChangesAsync();
        }

        return new KeyBundleResponseDto(bundle.DeviceId, bundle.RegistrationId, bundle.IdentityKey, bundle.SignedPreKey, bundle.SignedPreKeySig, oneTime);
    }

    public async Task<bool> DeleteDeviceAsync(Guid userId, string deviceId)
    {
        var bundle = await _db.KeyBundles!.FirstOrDefaultAsync(k => k.UserId == userId && k.DeviceId == deviceId);
        if (bundle == null)
            return false;

        // remove one-time prekeys for this bundle
        var otps = _db.OneTimePreKeys!.Where(p => p.KeyBundleId == bundle.Id);
        _db.OneTimePreKeys!.RemoveRange(otps);
        _db.KeyBundles!.Remove(bundle);
        await _db.SaveChangesAsync();
        return true;
    }
}
