using Microsoft.AspNetCore.Builder;
using back_end.Features.Users;

namespace back_end.Endpoints;

public static class EndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        // Feature endpoint mappings
        app.MapUsers();

        // Add other feature mappings here, e.g.:
        // app.MapChats();
    }
}
