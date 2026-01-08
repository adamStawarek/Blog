using Blog.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Blog.Infrastructure.Database.Interceptors;

internal sealed class AuditInterceptor : SaveChangesInterceptor
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

        var (user, date) = eventData.Context.Database.GetService<AuditContext>();

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { Entity: IAudit audit })
            {
                continue;
            }

            if (entry.State is EntityState.Added)
            {
                audit.SetCreated(user, date);
            }

            if (entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            {
                audit.SetUpdated(user, date);
            }
        }
    }
}

public sealed record AuditContext(string User, DateTime Date);