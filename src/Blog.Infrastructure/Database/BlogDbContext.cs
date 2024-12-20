using Blog.Domain.Entities;
using Blog.Infrastructure.Database.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blog.Infrastructure.Database;
public sealed class BlogDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private static readonly IReadOnlyList<IInterceptor> Interceptors = new[]
    {
        (IInterceptor)new SoftDeleteInterceptor(), (IInterceptor)new AuditInterceptor()
    };

    public DbSet<User> User { get; set; }
    public DbSet<Article> Article { get; set; }
    public DbSet<Comment> Comment { get; set; }

    public BlogDbContext(DbContextOptions<BlogDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);

        var defaultSchema = "dbo";
        modelBuilder.Entity<Article>(entity => entity.ToTable("Articles", defaultSchema));
        modelBuilder.Entity<Comment>(entity => entity.ToTable("Comments", defaultSchema));

        var identitySchema = "identity";
        modelBuilder.Entity<User>(entity => entity.ToTable("Users", identitySchema));
        modelBuilder.Entity<IdentityRole<Guid>>(entity => entity.ToTable("Roles", identitySchema));
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UserRoles", identitySchema));
        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UserClaims", identitySchema));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UserLogins", identitySchema));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RoleClaims", identitySchema));
        modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens", identitySchema));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(Interceptors);
    }
}

