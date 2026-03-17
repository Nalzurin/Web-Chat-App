using System;

namespace back_end.Features.Users.Dtos;

public record UserDto(Guid Id, string Username, string Email, DateTime CreatedAt);
