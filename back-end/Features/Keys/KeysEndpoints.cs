using back_end.Features.Keys.Dto;
using back_end.Features.Keys.Interfaces;

namespace back_end.Features.Keys;

public static class KeysEndpoints
{
    public static void MapKeys(this WebApplication app)
    {
        app.MapPost("/keys/upload", async (KeyUploadDto dto, IKeyRepository repo, System.Security.Claims.ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            await repo.UploadKeyBundleAsync(userId, dto);
            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/keys/{userId}", async (Guid userId, IKeyRepository repo) =>
        {
            var bundle = await repo.GetAndConsumeOneTimePreKeyAsync(userId);
            if (bundle == null)
                return Results.NotFound();
            return Results.Ok(bundle);
        }).RequireAuthorization();

        app.MapDelete("/keys/{userId}/{deviceId}", async (Guid userId, string deviceId, IKeyRepository repo, System.Security.Claims.ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var requester))
                return Results.Unauthorized();

            if (requester != userId)
                return Results.Forbid();

            var ok = await repo.DeleteDeviceAsync(userId, deviceId);
            return ok ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();
    }
}
