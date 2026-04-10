using MediatR;

namespace back_end.Features.Friendships.Requests;

public record AcceptFriendRequest(Guid FriendshipId, Guid ResponderId) : IRequest<bool>;

public class AcceptFriendRequestHandler : IRequestHandler<AcceptFriendRequest, bool>
{
    private readonly IFriendshipService _service;

    public AcceptFriendRequestHandler(IFriendshipService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(AcceptFriendRequest request, CancellationToken cancellationToken)
    {
        return await _service.AcceptRequestAsync(request.FriendshipId, request.ResponderId);
    }
}
