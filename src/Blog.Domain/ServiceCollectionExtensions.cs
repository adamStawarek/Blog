using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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

public interface IDomainService
{

}