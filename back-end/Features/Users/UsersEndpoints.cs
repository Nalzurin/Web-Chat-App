
using back_end.Features.Users.Interfaces;
using MediatR;

namespace back_end.Features.Users;

public static class UsersEndpoints
{
    public static void MapUsers(this WebApplication app)
    {
        app.MapGet("/users", async (IMediator mediator) => Results.Ok(await mediator.Send(new Requests.GetUsersQuery()))).WithName("GetUsers");

        app.MapPost("/users", async (CreateUser.Command cmd, IMediator mediator) =>
        {
            try
            {
                var created = await mediator.Send(cmd);
                return Results.Created($"/users/{created.Id}", created);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }).WithName("AddUser");

        app.MapPost("/login", async (Login.Command cmd, IMediator mediator) =>
        {
            try
            {
                var authResult = await mediator.Send(cmd);
                return Results.Ok(authResult);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }).WithName("Login");
    }

}
