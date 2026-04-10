using back_end.Models;

namespace back_end.Features.Friendships.Interfaces;

public interface IFriendshipRepository
{
    Task<Friendship?> GetAsync(Guid id);
    Task<List<Friendship>> GetForUserAsync(Guid userId);
    Task<Friendship> CreateAsync(Friendship f);
    Task<bool> UpdateStatusAsync(Guid id, FriendshipStatus status);
    Task<bool> DeleteAsync(Guid id);
}
