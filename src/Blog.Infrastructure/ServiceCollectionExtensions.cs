using Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlogInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<SqlServerDbContextOptionsBuilder>? action = null)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            var connString = configuration.GetConnectionString(nameof(BlogDbContext));

            options.UseSqlServer(connString, builder =>
            {
                action?.Invoke(builder);

                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            options.EnableDetailedErrors();
        });

        return services;
    }
}