
using back_end.Features.Users.Interfaces;

namespace back_end.Features.Users;

public static class UsersEndpoints
{
    public static void MapUsers(this WebApplication app)
    {
        app.MapGet("/users", async (IUsersService usersService) =>
        {
            var users = await usersService.GetUsersAsync();
            return Results.Ok(users);
        }).WithName("GetUsers");

        app.MapPost("/users", async (CreateUser.Command cmd, IUsersService usersService) =>
        {
            try
            {
                var created = await usersService.CreateUserAsync(cmd);
                return Results.Created($"/users/{created.Id}", created);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }).WithName("AddUser");
    }
}
