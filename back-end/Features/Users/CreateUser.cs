using MediatR;
using back_end.Features.Users.Dtos;

namespace back_end.Features.Users;

public static class CreateUser
{
    // Password is provided in plain text and will be hashed by the service
    public record Command(string Username, string Password, string Email) : IRequest<UserDto>;
}
