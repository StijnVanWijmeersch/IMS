using IMS.Application.Users;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Users.Commands.Create;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<Result<UserDto>>;
