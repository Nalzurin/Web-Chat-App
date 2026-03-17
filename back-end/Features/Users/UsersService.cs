using System;
using System.Linq;
using back_end.Models;
using back_end.Features.Users.Dtos;
using back_end.Features.Users.Interfaces;

namespace back_end.Features.Users;

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
        var users = await _repo.GetAllAsync();

        if (users.Any(u => string.Equals(u.Username, cmd.Username, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Username already exists.");
        }

        if (users.Any(u => string.Equals(u.Email, cmd.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Email already exists.");
        }

        var user = new User
        {
            Username = cmd.Username,
            Email = cmd.Email,
            PasswordHash = string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.CreateAsync(user, cmd.Password);

        return new UserDto(created.Id, created.Username, created.Email, created.CreatedAt);
    }
}
