using Blog.Application.Services.ApplicationUser;
using Blog.Application.Services.CurrentTime;
using Blog.Infrastructure.Database.Interceptors;
using Blog.Server.Services.Email;
using Blog.Server.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Blog.Server.Extensions;
public static class WebInstaller
{
    public static IServiceCollection AddBlogWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(serviceProvider =>
        {
            var currentTimeProvider = serviceProvider.GetRequiredService<ICurrentTimeProvider>();
            var currentUserProvider = serviceProvider.GetRequiredService<IApplicationUserProvider>();
            var currentUser = currentUserProvider.GetAsync().Result;
            return new AuditContext(currentUser.UserName, currentTimeProvider.Now().DateTime);
        });

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddTransient<IEmailSender, SendGridEmailSender>();
        services.Configure<EmailConfiguration>(configuration.GetSection(EmailConfiguration.Key));

        return services;
    }
}