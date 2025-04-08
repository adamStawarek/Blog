using Blog.Application.Services.FileStorage;
using Blog.Infrastructure.Database;
using Blog.Infrastructure.FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlogDatabase(this IServiceCollection services, IConfiguration configuration, Action<SqlServerDbContextOptionsBuilder>? action = null)
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

    public static IServiceCollection AddBlogFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleDriveFileStorageOptions>(configuration.GetSection(GoogleDriveFileStorageOptions.Key!));
        services.AddScoped<IFileStorage, GoogleDriveFileStorage>();

        return services;
    }
}