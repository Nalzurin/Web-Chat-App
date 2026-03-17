using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Models;

namespace back_end.Features.Users.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user, string password);
}
