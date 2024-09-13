using IMS.SharedKernel;

namespace IMS.Domain.Orders;

public class StatusErrors
{
    public static Error CreateStatusFailed(string name, string message) =>
        new Error("StatusErrors.CreateStatusFailed", $"Failed to create status with name {name}. {message}");

    public static Error StatusAlreadyExists(Name name) =>
        new Error("StatusErrors.StatusAlreadyExists", $"Status with name {name} already exists.");

    public static Error StatusNotFound(ulong statusId) =>
        new Error("StatusErrors.StatusNotFound", $"Status with id {statusId} not found.");
}
