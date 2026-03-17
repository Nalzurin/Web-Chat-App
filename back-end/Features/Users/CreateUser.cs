namespace back_end.Features.Users;

public static class CreateUser
{
    public record Command(string Username, string PasswordHash, string Email);
}
