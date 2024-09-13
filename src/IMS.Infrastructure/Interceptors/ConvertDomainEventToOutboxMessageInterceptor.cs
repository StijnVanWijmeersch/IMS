using IMS.Domain.Outbox;
using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace IMS.Infrastructure.Interceptors;

public sealed class ConvertDomainEventToOutboxMessageInterceptor : SaveChangesInterceptor
{

    // Intercepts the SaveChanges method of the DbContext to convert domain events to outbox messages.
    // The outbox pattern is used to ensure that messages are not lost in case of a failure.
    // We push the outbox messages to the database in the same transaction, so we can handle the event later on in the background.
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {

        var currentContext = eventData.Context;

        if (currentContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var messages = currentContext.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .SelectMany(e => e.PopDomainEvents())
            .Select(domainEvent => OutboxMessage.Create(
                     domainEvent.GetType().Name,
                     JsonConvert.SerializeObject(
                        domainEvent,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        })
                     ))
            .ToList();

        currentContext.Set<OutboxMessage>().AddRange(messages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
