using Blog.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Database.EntityConfigurations.Base;
public abstract class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityBase<T>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, guidValue => new Entity<T, Guid>.EntityId(guidValue))
            .ValueGeneratedOnAdd();

        builder.HasQueryFilter(x => x.Meta_IsActive);
    }
}