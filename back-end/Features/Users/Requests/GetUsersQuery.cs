using MediatR;
using back_end.Features.Users.Dtos;
using System.Collections.Generic;

namespace back_end.Features.Users.Requests;

public record GetUsersQuery() : IRequest<IEnumerable<UserDto>>;
