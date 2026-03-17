using MediatR;
using back_end.Features.Users.Dtos;
using back_end.Features.Users.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace back_end.Features.Users.Handlers;

public class LoginHandler : IRequestHandler<Login.Command, AuthResult>
{
    private readonly IUsersService _usersService;

    public LoginHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<AuthResult> Handle(Login.Command request, CancellationToken cancellationToken)
    {
        return await _usersService.AuthenticateAsync(request);
    }
}
