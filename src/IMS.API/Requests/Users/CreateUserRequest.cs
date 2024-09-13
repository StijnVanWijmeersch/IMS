namespace IMS.API.Requests.Users;

internal sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);
