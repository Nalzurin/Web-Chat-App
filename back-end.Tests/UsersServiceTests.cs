using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using back_end.Features.Users;
using back_end.Features.Users.Dtos;
using back_end.Features.Users.Interfaces;
using back_end.Models;

namespace back_end.Tests
{
    public class UsersServiceTests
    {
        [Fact]
        public async Task CreateUserAsync_WhenUsernameExists_ThrowsArgumentException()
        {
            var existing = new List<User> { new User { Username = "alice", Email = "alice@old.com" } };
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(existing);

            var svc = new UsersService(repoMock.Object);

            var cmd = new CreateUser.Command("alice", "Password123!", "alice@example.com");

            await Assert.ThrowsAsync<ArgumentException>(async () => await svc.CreateUserAsync(cmd));
        }

        [Fact]
        public async Task CreateUserAsync_WhenEmailExists_ThrowsArgumentException()
        {
            var existing = new List<User> { new User { Username = "old", Email = "bob@example.com" } };
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(existing);

            var svc = new UsersService(repoMock.Object);

            var cmd = new CreateUser.Command("bob", "Password123!", "bob@example.com");

            await Assert.ThrowsAsync<ArgumentException>(async () => await svc.CreateUserAsync(cmd));
        }

        [Fact]
        public async Task CreateUserAsync_WhenValid_CreatesUserAndReturnsDto()
        {
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            repoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync((User u, string p) =>
                {
                    u.Id = Guid.NewGuid();
                    u.PasswordHash = "HASHED";
                    return u;
                });

            var svc = new UsersService(repoMock.Object);

            var cmd = new CreateUser.Command("charlie", "Password123!", "charlie@example.com");

            var result = await svc.CreateUserAsync(cmd);

            Assert.Equal("charlie", result.Username);
            Assert.Equal("charlie@example.com", result.Email);
            Assert.NotEqual(Guid.Empty, result.Id);
            repoMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}
