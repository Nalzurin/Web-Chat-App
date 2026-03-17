using System;

namespace back_end.Features.Users.Dtos;

public record AuthResult(string Token, UserDto User);
