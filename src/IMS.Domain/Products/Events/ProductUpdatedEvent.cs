using IMS.SharedKernel;

namespace IMS.Domain.Products.Events;

public sealed record ProductUpdatedEvent(Product UpdatedProduct) : IDomainEvent;