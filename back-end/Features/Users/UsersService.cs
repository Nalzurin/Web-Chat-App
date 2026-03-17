using System.Linq;
using back_end.Models;

namespace back_end.Features.Users;

public interface IUsersService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUser.Command cmd);
}

public class UsersService : IUsersService
{
    private readonly IUserRepository _repo;

    public UsersService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.Username, u.Email, u.CreatedAt));
    }

    public async Task<UserDto> CreateUserAsync(CreateUser.Command cmd)
    {
        var user = new User
        {
            Username = cmd.Username,
            Email = cmd.Email,
            PasswordHash = cmd.PasswordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(user);
        await _repo.SaveChangesAsync();

        return new UserDto(user.Id, user.Username, user.Email, user.CreatedAt);
    }
}
