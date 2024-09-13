using IMS.SharedKernel;

namespace IMS.Domain.Customers;

public class CustomerErrors
{
    public static Error CreateCustomerFailed(string firstName, string lastName, string message) =>
        new Error("CustomerErrors.CreateCustomerFailed", $"Failed to create customer: {firstName} {lastName}. {message}");

    public static Error CustomerNotFound(ulong id) =>
        new Error("CustomerErrors.CustomerNotFound", $"Customer with id: {id} does not exist");

    public static Error EmailIsNotUnique(string email) =>
        new Error("CustomerErrors.EmailIsNotUnique", $"Email: {email} is already in use");

    public static Error RemoveCustomerFailed(ulong customerId, string message) =>
        new Error("CustomerErrors.RemoveCustomerFailed", $"Failed to remove customer with id {customerId}. {message}");

    public static Error UpdateCustomerFailed(ulong customerId, string message) =>
        new Error("CustomerErrors.UpdateCustomerFailed", $"Failed to update customer with id {customerId}. {message}");
}
