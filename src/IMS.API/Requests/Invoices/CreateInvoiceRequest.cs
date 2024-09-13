namespace IMS.API.Requests.Invoices;

public sealed record CreateInvoiceRequest
{
    public ulong OrderId { get; init; }
    public ulong StatusId { get; init; }
}
