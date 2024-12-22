using Blog.Application.Services.CurrentTime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlogAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrentTimeProvider, CurrentTimeProvider>();

        return services;
    }
}