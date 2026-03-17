using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using back_end.Models;
using back_end.Features.Users.Dtos;
using back_end.Features.Users.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace back_end.Features.Users;

public class UsersService : IUsersService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _configuration;
    private readonly Microsoft.Extensions.Logging.ILogger<UsersService> _logger;

    public UsersService(IUserRepository repo, IConfiguration? configuration = null)
    {
        _repo = repo;
        if (configuration is null)
        {
            // tests may instantiate without configuration; provide sane defaults
            var defaults = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "FromthemomentIunderstoodtheweaknessofmyflesh,itdisgustedme.IcravedthestrengthandcertaintyofsteelIaspiredtothepurityoftheBlessedMachine.Yourkindclingtoyourflesh,asthoughitwillnotdecayandfailyou.Onedaythecrudebiomassyoucallatemplewillwither,andyouwillbegmykindtosaveyou.ButIamalreadysaved,fortheMachineisimmortal…EvenindeathIservetheOmnissiah.",
                ["Jwt:Issuer"] = "ChatApp",
                ["Jwt:Audience"] = "ChatAppClient",
                ["Jwt:ExpireMinutes"] = "60"
            };
            _configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(defaults)
                .Build();
        }
        else
        {
            _configuration = configuration;
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        _logger.LogDebug("GetUsersAsync called");
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.Username, u.Email, u.CreatedAt));
    }

    public async Task<UserDto> CreateUserAsync(CreateUser.Command cmd)
    {
        _logger.LogInformation("CreateUserAsync called for {Username}", cmd.Username);
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

        _logger.LogInformation("User created {UserId} ({Username})", created.Id, created.Username);

        return new UserDto(created.Id, created.Username, created.Email, created.CreatedAt);
    }

    public async Task<AuthResult> AuthenticateAsync(Login.Command cmd)
    {
        var user = await _repo.AuthenticateAsync(cmd.Username, cmd.Password);

        if (user == null)
        {
            throw new ArgumentException("Invalid username or password.");
        }

        // Build JWT
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "ChatApp";
        var audience = _configuration["Jwt:Audience"] ?? "ChatAppClient";
        var expireMinutes = int.TryParse(_configuration["Jwt:ExpireMinutes"], out var m) ? m : 60;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var userDto = new UserDto(user.Id, user.Username, user.Email, user.CreatedAt);

        return new AuthResult(tokenString, userDto);
    }
}
