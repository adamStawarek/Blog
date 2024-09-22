using Blog.Domain.Entities;
using Blog.Infrastructure.Database.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Database.EntityConfigurations;
internal sealed class UserConfiguration : EntityBaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.DisplayName).HasMaxLength(200);

        builder.Property(x => x.FirstName).HasMaxLength(200);

        builder.Property(x => x.LastName).HasMaxLength(200);

        builder.HasIndex(x => x.DisplayName);
    }
}