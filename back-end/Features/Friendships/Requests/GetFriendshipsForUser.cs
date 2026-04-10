using MediatR;
using back_end.Models;

namespace back_end.Features.Friendships.Requests;

public record GetFriendshipsForUser(Guid UserId) : IRequest<List<Friendship>>;

public class GetFriendshipsForUserHandler : IRequestHandler<GetFriendshipsForUser, List<Friendship>>
{
    private readonly IFriendshipService _service;

    public GetFriendshipsForUserHandler(IFriendshipService service)
    {
        _service = service;
    }

    public async Task<List<Friendship>> Handle(GetFriendshipsForUser request, CancellationToken cancellationToken)
    {
        return await _service.GetForUserAsync(request.UserId);
    }
}
