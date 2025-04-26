using Blog.Application.Services.Email;
using Blog.Application.Services.FileStorage;
using Blog.Application.Services.Jobs;
using Blog.Domain.Entities.Base;
using Blog.Infrastructure.Database;
using Blog.Infrastructure.Email;
using Blog.Infrastructure.FileStorage;
using Blog.Infrastructure.Jobs;
using Hangfire;
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

        services.AddScoped<IContext, BlogDbContext>();

        return services;
    }

    public static IServiceCollection AddBlogFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleDriveFileStorageOptions>(configuration.GetSection(GoogleDriveFileStorageOptions.Key!));
        services.AddScoped<IFileStorage, GoogleDriveFileStorage>();

        return services;
    }

    public static IServiceCollection AddBlogEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SendGridEmailSenderOptions>(configuration.GetSection(SendGridEmailSenderOptions.Key!));
        services.AddTransient<IEmailSender, SendGridEmailSender>();

        return services;
    }

    public static IServiceCollection AddBlogBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HangfireBackgroundJobProcessorOptions>(configuration.GetSection(HangfireBackgroundJobProcessorOptions.Key!));
        services.AddScoped<IBackgroundJobProcessor, HangfireBackgroundJobProcessor>();

        services.AddHangfire(x =>
        {
            x.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireDbContext"));
        });


        return services;
    }

    public static IServiceCollection AddBlogBackgroundServicesProcessor(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer();

        return services;
    }
}