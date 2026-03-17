using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using back_end.Models;

namespace back_end.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // Identity configuration
        services.AddIdentity<User, IdentityRole<Guid>>(o =>
        {
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireUppercase = false;
            o.Password.RequiredLength = 8;

        }).AddEntityFrameworkStores<back_end.Data.ChatDbContext>()
          .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Authentication
        var jwtKey = configuration["Jwt:Key"] ?? "ReplaceWithASecretKeyForDevelopmentOnly_ShouldBeLong";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "ChatApp";
        var jwtAudience = configuration["Jwt:Audience"] ?? "ChatAppClient";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            // Allow SignalR websocket requests to send the access token as a query string parameter
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"].FirstOrDefault();
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                ,
                // Make sure SignalR's Context.UserIdentifier will be set to the token "sub" claim
                NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub
            };
        });


        return services;
    }

    // Backwards-compatible convenience method
    public static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityServices();
        services.AddJwtAuthentication(configuration);
        return services;
    }
}
