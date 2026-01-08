using Blog.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blog.Infrastructure.Database.Interceptors;

internal class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        MarkMetaProperties(eventData);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        MarkMetaProperties(eventData);

        return base.SavingChanges(eventData, result);
    }

    private static void MarkMetaProperties(DbContextEventData eventData)
    {
        if (eventData.Context is null)
        {
            return;
        }

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { Entity: ISoftDelete delete })
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    delete.Activate();
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    delete.Deactivate();
                    break;
                default:
                    continue;
            }
        }
    }
}