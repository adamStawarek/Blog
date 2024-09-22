using Blog.Domain.Entities;
using Blog.Infrastructure.Database.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure.Database.EntityConfigurations;
internal sealed class CommentConfiguration : EntityBaseConfiguration<Comment>
{
    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.HasIndex(x => x.Meta_CreatedDate).IsDescending();

        builder.Property(x => x.AuthorId)
            .HasConversion(id => id.Value, guidValue => new User.EntityId(guidValue));

        builder.Property(x => x.ArticleId)
            .HasConversion(id => id.Value, guidValue => new Article.EntityId(guidValue));

        builder.HasOne(x => x.Article)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Author)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}