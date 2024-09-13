using IMS.Domain.Products;
using IMS.SharedKernel;

namespace IMS.Domain.Categories.Events;

public sealed record CategoryUpdatedEvent(Name Name, Category UpdatedCategory) : IDomainEvent;
