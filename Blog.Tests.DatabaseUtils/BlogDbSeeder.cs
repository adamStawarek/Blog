using Blog.Domain.Interfaces;
using Blog.Infrastructure.Database;

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

        return this;
    }
}