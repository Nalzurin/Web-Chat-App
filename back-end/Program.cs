
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Features.Users;
using back_end.Features.Users.Interfaces;
using back_end.Endpoints;
using back_end.Extensions;
using back_end.Features.Friendships;
using MediatR;
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
            builder.Services.AddSignalR();

            // Allow the frontend dev server to call the API (adjust origins as needed)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCors", policy =>
                    policy.WithOrigins("http://localhost:15912")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            // Infrastructure and business/service registrations
            builder.Services.AddScoped<IUserRepository, Infrastructure.Users.UserRepository>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<Features.Keys.Interfaces.IKeyRepository, Infrastructure.Keys.KeyRepository>();
            builder.Services.AddScoped<Features.Friendships.Interfaces.IFriendshipRepository, Infrastructure.Friendships.FriendshipRepository>();
            builder.Services.AddScoped<Features.Friendships.IFriendshipService, Features.Friendships.FriendshipService>();

            // Identity and JWT registration (split for clarity)
            builder.Services.AddIdentityServices();
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // MediatR for CQRS/mediator pattern
            builder.Services.AddMediatR(typeof(Program).Assembly);

            // Register MediatR pipeline behaviors (logging)
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Infrastructure.Logging.LoggingBehavior<,>));
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            // Enable CORS for development
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("DevCors");
            }

            app.UseAuth();

            // Map hubs and vertical-slice feature endpoints
            app.MapHub<Hubs.ChatHub>("/hubs/chat");
            app.MapEndpoints();
            app.MapFriendships();

            app.Run();
        }
    }
}
