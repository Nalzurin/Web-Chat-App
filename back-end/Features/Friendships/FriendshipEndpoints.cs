using MediatR;
using back_end.Features.Friendships.Requests;
using back_end.Features.Friendships;

namespace back_end.Features.Friendships;

public static class FriendshipEndpoints
{
    public static void MapFriendships(this WebApplication app)
    {
        app.MapPost("/friendships/request", async (Guid addresseeId, IMediator mediator, System.Security.Claims.ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var requester)) return Results.Unauthorized();
            var created = await mediator.Send(new SendFriendRequest(requester, addresseeId));
            return Results.Created($"/friendships/{created.Id}", created);
        }).RequireAuthorization();

        app.MapPost("/friendships/{id:guid}/accept", async (Guid id, IMediator mediator, System.Security.Claims.ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var responder)) return Results.Unauthorized();
            var ok = await mediator.Send(new AcceptFriendRequest(id, responder));
            return ok ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        app.MapGet("/friendships", async (IMediator mediator, System.Security.Claims.ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return Results.Unauthorized();
            var list = await mediator.Send(new GetFriendshipsForUser(userId));
            return Results.Ok(list);
        }).RequireAuthorization();

        app.MapDelete("/friendships/{id:guid}", async (Guid id, IMediator mediator, System.Security.Claims.ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return Results.Unauthorized();
            var ok = await mediator.Send(new DeleteFriendship(id, userId));
            return ok ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();
    }
}
