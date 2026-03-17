using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Xunit;
using back_end.Data;
using back_end.Models;
using back_end.Infrastructure.Users;

namespace back_end.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_CreatesUser_And_GetAllReturnsIt()
        {
            // Arrange - build a fresh service provider with an isolated in-memory DB for the test
            var provider = CreateServiceProvider(Guid.NewGuid().ToString());

            using (var scope = provider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var repo = new UserRepository(userManager);

                var user = new User
                {
                    Username = "repouser",
                    Email = "repo@example.com",
                    CreatedAt = DateTime.UtcNow
                };

                // Act
                var created = await repo.CreateAsync(user, "Password123!");
                var all = await repo.GetAllAsync();

                // Assert
                Assert.Single(all);
                Assert.Equal(created.Email, all[0].Email);
                Assert.Equal(created.Username, all[0].Username);
            }
        }

        private static ServiceProvider CreateServiceProvider(string inMemoryDbName)
        {
            var services = new ServiceCollection();
            services.AddDbContext<ChatDbContext>(options =>
                options.UseInMemoryDatabase(inMemoryDbName));

            // Ensure logging is available for Identity components (e.g. UserManager<T>)
            services.AddLogging();

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ChatDbContext>()
                .AddDefaultTokenProviders();

            return services.BuildServiceProvider();
        }
    }
}
