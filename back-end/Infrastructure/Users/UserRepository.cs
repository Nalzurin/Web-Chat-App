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
    private readonly Microsoft.Extensions.Logging.ILogger<UserRepository> _logger;

    public UserRepository(UserManager<User> userManager, Microsoft.Extensions.Logging.ILogger<UserRepository>? logger = null)
    {
        _userManager = userManager;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<UserRepository>.Instance;
    }

    public async Task<List<User>> GetAllAsync()
    {
        _logger.LogDebug("GetAllAsync called");
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

        _logger.LogInformation("Creating user {Username}", appUser.UserName);
        var result = await _userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to create user {Username}: {Errors}", appUser.UserName, errors);
            // Throw ArgumentException so higher layers (endpoints) can return a 400 with the
            // Identity errors (for example password validation messages) in the response body.
            throw new ArgumentException(errors);
        }

        _logger.LogInformation("User {Username} created successfully (Id={UserId})", appUser.UserName, appUser.Id);

        return new User
        {
            Id = appUser.Id,
            Username = appUser.UserName ?? string.Empty,
            Email = appUser.Email ?? string.Empty,
            PasswordHash = appUser.PasswordHash ?? string.Empty,
            CreatedAt = appUser.CreatedAt
        };
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        _logger.LogDebug("AuthenticateAsync called for {Username}", username);
        // Find by username (case-sensitive per Identity) and verify password
        var appUser = await _userManager.FindByNameAsync(username);
        if (appUser == null)
        {
            _logger.LogWarning("AuthenticateAsync: user not found {Username}", username);
            return null;
        }

        var valid = await _userManager.CheckPasswordAsync(appUser, password);
        if (!valid)
        {
            _logger.LogWarning("AuthenticateAsync: invalid password for {Username}", username);
            return null;
        }

        _logger.LogInformation("AuthenticateAsync: user {Username} authenticated", username);

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
