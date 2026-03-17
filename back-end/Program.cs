
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Features.Users;
using back_end.Features.Users.Interfaces;
using back_end.Models;
using Microsoft.AspNetCore.Identity;
using back_end.Endpoints;

namespace back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Infrastructure and business/service registrations
            builder.Services.AddScoped<IUserRepository, Infrastructure.Users.UserRepository>();
            builder.Services.AddScoped<IUsersService, Features.Users.UsersService>();

            // Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ChatDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Map vertical-slice feature endpoints
            app.MapEndpoints();

            app.Run();
        }
    }
}
