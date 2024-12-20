using Blog.Domain.Entities;
using Blog.Infrastructure.Database.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Database.EntityConfigurations;
internal sealed class ArticleConfiguration : EntityBaseConfiguration<Article>
{
    public override void Configure(EntityTypeBuilder<Article> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title).HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(400);

        builder.Property(x => x.Content).HasMaxLength(4000);

        builder.HasIndex(x => x.Title).IsUnique();

        builder.HasIndex(x => x.Meta_CreatedDate).IsDescending();

        builder.HasOne(x => x.Author)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}