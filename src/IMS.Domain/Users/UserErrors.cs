using IMS.SharedKernel;

namespace IMS.Domain.Users;

public class UserErrors
{
    public static Error CreateUserFailed(string firstName, string lastName, string message) =>
        new Error("UserErrors.CreateUserFailed", $"Failed to create user: {firstName} {lastName}. {message}");

    public static Error EmailIsNotUnique(string email) =>
        new Error("UserErrors.EmailIsNotUnique", $"Email: {email} is already in use");
}
