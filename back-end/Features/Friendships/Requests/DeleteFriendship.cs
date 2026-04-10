using MediatR;

namespace back_end.Features.Friendships.Requests;

public record DeleteFriendship(Guid FriendshipId, Guid RequesterId) : IRequest<bool>;

public class DeleteFriendshipHandler : IRequestHandler<DeleteFriendship, bool>
{
    private readonly IFriendshipService _service;

    public DeleteFriendshipHandler(IFriendshipService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(DeleteFriendship request, CancellationToken cancellationToken)
    {
        return await _service.RemoveAsync(request.FriendshipId, request.RequesterId);
    }
}
