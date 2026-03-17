using System;
using System.Linq;
using System;
using System.Linq;
using back_end.Features.Users.Interfaces;
using back_end.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace back_end.Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<User>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        return users.Select(u => new User
        {
            Id = u.Id,
            Username = u.UserName ?? string.Empty,
            Email = u.Email ?? string.Empty,
            PasswordHash = u.PasswordHash ?? string.Empty,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        var appUser = new User
        {
            Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id,
            UserName = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        var result = await _userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        return new User
        {
            Id = appUser.Id,
            Username = appUser.UserName ?? string.Empty,
            Email = appUser.Email ?? string.Empty,
            PasswordHash = appUser.PasswordHash ?? string.Empty,
            CreatedAt = appUser.CreatedAt
        };
    }
}
