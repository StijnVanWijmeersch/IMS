namespace IMS.API.Requests.Customer;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email);
