using Microsoft.AspNetCore.Builder;

namespace back_end.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
