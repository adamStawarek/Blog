using Blog.Infrastructure.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blog.Infrastructure.Database;
public sealed class BlogDbContext : DbContext
{
    private static readonly IReadOnlyList<IInterceptor> Interceptors = new[]
    {
        (IInterceptor)new SoftDeleteInterceptor(), (IInterceptor)new AuditInterceptor()
    };

    public BlogDbContext(DbContextOptions<BlogDbContext> dbContextOptions) : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(Interceptors);
    }
}