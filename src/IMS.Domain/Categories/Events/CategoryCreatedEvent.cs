using IMS.SharedKernel;

namespace IMS.Domain.Categories.Events;

public sealed record CategoryCreatedEvent(Category NewCategory) : IDomainEvent;
