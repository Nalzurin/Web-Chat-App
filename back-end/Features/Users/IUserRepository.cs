using back_end.Models;

namespace back_end.Features.Users;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
