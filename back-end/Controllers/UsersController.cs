using back_end.Data;
using back_end.Models;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {

        [HttpGet(Name = "GetUsers")]
        public IEnumerable<User> Get(ChatDbContext _dbContext)
        {
            return _dbContext.Users;
        }
        [HttpPost(Name = "AddUser")]
        public User AddUser(ChatDbContext _dbContext, [FromBody] UserDto _newUser)
        {
            User newUser = new()
            {
                Username = _newUser.username,
                Email = _newUser.email,
                PasswordHash = _newUser.passwordhash,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            return newUser;
        }
    }
    public record UserDto(string username, string passwordhash, string email);
}
