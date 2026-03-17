using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using back_end.Data;
using back_end.Features.Keys.Dto;
using back_end.Infrastructure.Keys;

namespace back_end.Tests
{
    public class KeyRepositoryTests
    {
        private static ChatDbContext CreateContext(string name)
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ChatDbContext(options);
        }

        [Fact]
        public async Task UploadKeyBundleAsync_CreatesBundle_AndStoresPreKeys()
        {
            var userId = Guid.NewGuid();
            using var db = CreateContext(Guid.NewGuid().ToString());
            var repo = new KeyRepository(db);

            var dto = new KeyUploadDto("device1", 42, "identity", "signed", "sig", new[] { "pk1", "pk2" });

            await repo.UploadKeyBundleAsync(userId, dto);

            var bundle = await db.KeyBundles.Include(k => k.OneTimePreKeys).FirstOrDefaultAsync(k => k.UserId == userId && k.DeviceId == "device1");

            Assert.NotNull(bundle);
            Assert.Equal("device1", bundle.DeviceId);
            Assert.Equal(42, bundle.RegistrationId);
            Assert.Equal(2, bundle.OneTimePreKeys.Count);
            Assert.Contains(bundle.OneTimePreKeys, p => p.PreKey == "pk1");
            Assert.Contains(bundle.OneTimePreKeys, p => p.PreKey == "pk2");
        }

        [Fact]
        public async Task GetAndConsumeOneTimePreKeyAsync_ReturnsOneTimePreKey_AndRemovesIt()
        {
            var userId = Guid.NewGuid();
            using var db = CreateContext(Guid.NewGuid().ToString());
            var repo = new KeyRepository(db);

            var dto = new KeyUploadDto("device1", 1, "identity", "signed", "sig", new[] { "pkA", "pkB" });
            await repo.UploadKeyBundleAsync(userId, dto);

            var beforeCount = await db.OneTimePreKeys.CountAsync();
            Assert.Equal(2, beforeCount);

            var bundle = await repo.GetAndConsumeOneTimePreKeyAsync(userId);

            Assert.NotNull(bundle);
            Assert.NotNull(bundle.OneTimePreKey);
            var afterCount = await db.OneTimePreKeys.CountAsync();
            Assert.Equal(1, afterCount);

            // subsequent call should return the remaining OTP or null if none
            var bundle2 = await repo.GetAndConsumeOneTimePreKeyAsync(userId);
            Assert.NotNull(bundle2);
            var afterCount2 = await db.OneTimePreKeys.CountAsync();
            Assert.Equal(0, afterCount2);
        }

        [Fact]
        public async Task DeleteDeviceAsync_RemovesBundleAndPreKeys()
        {
            var userId = Guid.NewGuid();
            using var db = CreateContext(Guid.NewGuid().ToString());
            var repo = new KeyRepository(db);

            var dto = new KeyUploadDto("device-to-delete", 7, "identity", "signed", "sig", new[] { "x1" });
            await repo.UploadKeyBundleAsync(userId, dto);

            var bundle = await db.KeyBundles.Include(k => k.OneTimePreKeys).FirstOrDefaultAsync(k => k.UserId == userId && k.DeviceId == "device-to-delete");
            Assert.NotNull(bundle);
            Assert.Single(bundle.OneTimePreKeys);

            var deleted = await repo.DeleteDeviceAsync(userId, "device-to-delete");
            Assert.True(deleted);

            var bundleAfter = await db.KeyBundles.FirstOrDefaultAsync(k => k.UserId == userId && k.DeviceId == "device-to-delete");
            Assert.Null(bundleAfter);

            var otps = await db.OneTimePreKeys.Where(p => p.KeyBundleId == bundle.Id).ToListAsync();
            Assert.Empty(otps);
        }
    }
}
