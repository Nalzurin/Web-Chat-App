using Microsoft.AspNetCore.Builder;
using back_end.Features.Users;
using back_end.Features.Keys;

namespace back_end.Endpoints;

public static class EndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        // Feature endpoint mappings
        app.MapUsers();
        app.MapKeys();

        // Add other feature mappings here, e.g.:
        // app.MapChats();
    }
}
