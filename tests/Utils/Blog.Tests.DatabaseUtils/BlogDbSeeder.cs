using Blog.Domain.Entities.Base;
using Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Blog.Tests.DatabaseUtils;
public class BlogDbSeeder
{
    private readonly BlogDbContext _context;

    private BlogDbSeeder(BlogDbContext context)
    {
        _context = context;
    }

    public static BlogDbSeeder Create(BlogDbContext context) => new(context);

    public BlogDbSeeder Add<T>(Action<T> action, out T entity) where T : class, IEntity, new()
    {
        entity = new T();

        var defaultSeeder = BlogDataGenerator.Generate<T>();

        defaultSeeder(entity);

        action(entity);

        _context.Set<T>().Add(entity);

        _context.SaveChanges();

        // Detach the entity to ensure fresh data is fetched from the database
        _context.Entry(entity).State = EntityState.Detached;

        return this;
    }
}