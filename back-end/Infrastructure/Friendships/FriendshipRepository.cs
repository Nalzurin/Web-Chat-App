using back_end.Data;
using back_end.Features.Friendships.Interfaces;
using back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace back_end.Infrastructure.Friendships;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly ChatDbContext _db;

    public FriendshipRepository(ChatDbContext db)
    {
        _db = db;
    }

    public async Task<Friendship?> GetAsync(Guid id)
    {
        return await _db.Friendships!.FindAsync(id);
    }

    public async Task<List<Friendship>> GetForUserAsync(Guid userId)
    {
        return await _db.Friendships!.Where(f => f.RequesterId == userId || f.AddresseeId == userId).ToListAsync();
    }

    public async Task<Friendship> CreateAsync(Friendship f)
    {
        f.Id = f.Id == Guid.Empty ? Guid.NewGuid() : f.Id;
        _db.Friendships!.Add(f);
        await _db.SaveChangesAsync();
        return f;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, FriendshipStatus status)
    {
        var f = await _db.Friendships!.FindAsync(id);
        if (f == null) return false;
        f.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var f = await _db.Friendships!.FindAsync(id);
        if (f == null) return false;
        _db.Friendships!.Remove(f);
        await _db.SaveChangesAsync();
        return true;
    }
}
