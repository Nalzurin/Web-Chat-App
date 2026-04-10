using back_end.Features.Friendships.Interfaces;
using back_end.Models;

namespace back_end.Features.Friendships;

public interface IFriendshipService
{
    Task<Friendship> SendRequestAsync(Guid requesterId, Guid addresseeId);
    Task<bool> AcceptRequestAsync(Guid friendshipId, Guid responderId);
    Task<List<Friendship>> GetForUserAsync(Guid userId);
    Task<bool> RemoveAsync(Guid friendshipId, Guid requesterId);
}

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _repo;

    public FriendshipService(IFriendshipRepository repo)
    {
        _repo = repo;
    }

    public async Task<Friendship> SendRequestAsync(Guid requesterId, Guid addresseeId)
    {
        var f = new Friendship
        {
            RequesterId = requesterId,
            AddresseeId = addresseeId,
            Status = FriendshipStatus.Pending
        };
        return await _repo.CreateAsync(f);
    }

    public async Task<bool> AcceptRequestAsync(Guid friendshipId, Guid responderId)
    {
        var f = await _repo.GetAsync(friendshipId);
        if (f == null) return false;
        if (f.AddresseeId != responderId) return false;
        return await _repo.UpdateStatusAsync(friendshipId, FriendshipStatus.Accepted);
    }

    public async Task<List<Friendship>> GetForUserAsync(Guid userId)
    {
        return await _repo.GetForUserAsync(userId);
    }

    public async Task<bool> RemoveAsync(Guid friendshipId, Guid requesterId)
    {
        var f = await _repo.GetAsync(friendshipId);
        if (f == null) return false;
        if (f.RequesterId != requesterId && f.AddresseeId != requesterId) return false;
        return await _repo.DeleteAsync(friendshipId);
    }
}
