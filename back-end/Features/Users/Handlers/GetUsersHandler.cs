using MediatR;
using back_end.Features.Users.Dtos;
using back_end.Features.Users.Requests;
using back_end.Features.Users.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace back_end.Features.Users.Handlers;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUsersService _usersService;

    public GetUsersHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _usersService.GetUsersAsync();
    }
}
