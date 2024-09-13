using IMS.Application.Abstractions;
using IMS.Application.Mappers;
using IMS.Application.Users.Contracts;
using IMS.Domain.Customers;
using IMS.Domain.Users;
using IMS.SharedKernel;
using MediatR;

namespace IMS.Application.Users.Commands.Create;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var uniqueEmail = await _userRepository.IsEmailUniqueAsync(new Email(request.Email), cancellationToken);

        if (!uniqueEmail)
        {
            return Result.Failure<UserDto>(UserErrors.EmailIsNotUnique(request.Email));
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = User.Create(request.FirstName, request.LastName, request.Email, hashedPassword);

        try
        {
            _userRepository.Add(newUser);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(newUser.ToUserDto());
        }
        catch (Exception ex)
        {
            return Result.Failure<UserDto>(UserErrors.CreateUserFailed(request.FirstName, request.LastName, ex.Message));
        }
    }
}
