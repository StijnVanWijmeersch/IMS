namespace IMS.Application.Users;

public sealed record UserDto
{
    public ulong? Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
}
