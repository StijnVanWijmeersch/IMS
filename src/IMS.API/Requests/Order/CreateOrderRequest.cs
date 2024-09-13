namespace IMS.API.Requests.Order;

public sealed record CreateOrderRequest(
    ulong CustomerId,
    ulong StatusId,
    Dictionary<ulong, int> Products,
    List<ulong> Invoices
    );
