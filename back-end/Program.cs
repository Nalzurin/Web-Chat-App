
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Features.Users;
using back_end.Features.Users.Interfaces;
using back_end.Models;
using Microsoft.AspNetCore.Identity;
using back_end.Endpoints;
using back_end.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

namespace back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog early so startup logs are captured
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: Serilog.RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Infrastructure and business/service registrations
            builder.Services.AddScoped<IUserRepository, Infrastructure.Users.UserRepository>();
            builder.Services.AddScoped<IUsersService, Features.Users.UsersService>();
            builder.Services.AddScoped<back_end.Features.Keys.Interfaces.IKeyRepository, back_end.Infrastructure.Keys.KeyRepository>();

            // Identity and JWT registration (split for clarity)
            builder.Services.AddIdentityServices();
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // MediatR for CQRS/mediator pattern
            builder.Services.AddMediatR(typeof(Program).Assembly);

            // Register MediatR pipeline behaviors (logging)
            builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(back_end.Infrastructure.Logging.LoggingBehavior<,>));
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuth();

            // Map hubs and vertical-slice feature endpoints
            app.MapHub<back_end.Hubs.ChatHub>("/hubs/chat");
            app.MapEndpoints();

            app.Run();
        }
    }
}
