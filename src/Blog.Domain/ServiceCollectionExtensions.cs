using Blog.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Blog.Domain;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAcademyPlatformDomain(this IServiceCollection services)
    {
        services.Scan(scanner =>
            scanner.FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo<IDomainService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}