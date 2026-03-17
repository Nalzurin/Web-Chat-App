using MediatR;
using back_end.Features.Users.Dtos;

namespace back_end.Features.Users;

public static class Login
{
    // Plain-text credentials supplied by the client
    public record Command(string Username, string Password) : IRequest<AuthResult>;
}

