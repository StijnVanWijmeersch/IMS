using IMS.Domain.Products;
using IMS.SharedKernel;

namespace IMS.Domain.Categories.Events;

public sealed record CategoryRemovedEvent(Name Name) : IDomainEvent;
