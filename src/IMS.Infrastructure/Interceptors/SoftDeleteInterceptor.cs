using IMS.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IMS.Infrastructure.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{

    // Intercepts the SaveChanges method of the DbContext.
    // Transforms the EntityState of the entities that implement the ISoftDelete interface to Modified instead of Deleted and sets the IsDeleted property to true.
    // This way, the entity will not be physically deleted from the database. But it will be marked as deleted.
    // We will filter the deleted entities in the queries.
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var currentContext = eventData.Context;

        if (currentContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = currentContext.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is ISoftDelete entity &&
                entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

}
