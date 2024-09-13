using IMS.SharedKernel;

namespace IMS.Domain.Products.Events;

public sealed record ProductRemovedEvent(Product Product) : IDomainEvent;
