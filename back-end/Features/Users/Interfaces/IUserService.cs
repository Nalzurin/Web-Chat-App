using back_end.Features.Users.Dtos;

namespace back_end.Features.Users.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUser.Command cmd);
    }
}
