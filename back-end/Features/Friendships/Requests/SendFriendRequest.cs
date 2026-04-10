using MediatR;
using back_end.Models;

namespace back_end.Features.Friendships.Requests;

public record SendFriendRequest(Guid RequesterId, Guid AddresseeId) : IRequest<Friendship>;

public class SendFriendRequestHandler : IRequestHandler<SendFriendRequest, Friendship>
{
    private readonly IFriendshipService _service;

    public SendFriendRequestHandler(IFriendshipService service)
    {
        _service = service;
    }

    public async Task<Friendship> Handle(SendFriendRequest request, CancellationToken cancellationToken)
    {
        return await _service.SendRequestAsync(request.RequesterId, request.AddresseeId);
    }
}
