using Blog.Application.Services.ApplicationUser;
using Blog.Application.Services.CurrentTime;
using Blog.Clients.Web.Api.Validation;
using Blog.Infrastructure.Database.Interceptors;
using FluentValidation;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Blog.Clients.Web.Api.Installers;
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

        services.AddTransient<IEmailSender, MicrosoftIdentityEmailSenderAdapter>();

        return services;
    }
}

internal class MicrosoftIdentityEmailSenderAdapter : IEmailSender
{
    private readonly Application.Services.Email.IEmailSender _emailSender;

    public MicrosoftIdentityEmailSenderAdapter(Application.Services.Email.IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return _emailSender.SendAsync(email, subject, htmlMessage);
    }
}