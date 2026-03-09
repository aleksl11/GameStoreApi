namespace GameStore.Api.Dtos.Users;

public record UserDto(
    string Name,
    string Email,
    string Role
);