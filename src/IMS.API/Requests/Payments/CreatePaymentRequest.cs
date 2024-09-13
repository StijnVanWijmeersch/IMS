namespace IMS.API.Requests.Payments;

internal sealed record CreatePaymentRequest(ulong InvoiceId, decimal Amount);
