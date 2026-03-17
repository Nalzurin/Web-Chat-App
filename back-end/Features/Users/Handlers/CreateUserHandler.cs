using MediatR;
using back_end.Features.Users.Dtos;
using back_end.Features.Users.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace back_end.Features.Users.Handlers;

public class CreateUserHandler : IRequestHandler<CreateUser.Command, UserDto>
{
    private readonly IUsersService _usersService;

    public CreateUserHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<UserDto> Handle(CreateUser.Command request, CancellationToken cancellationToken)
    {
        return await _usersService.CreateUserAsync(request);
    }
}
