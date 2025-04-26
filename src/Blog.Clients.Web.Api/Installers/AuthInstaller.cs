using Blog.Application.Services.ApplicationUser;
using Blog.Clients.Web.Api.Auth;
using Blog.Clients.Web.Api.Services.ApplicationUser;
using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;

namespace Blog.Clients.Web.Api.Installers;
public static class AuthInstaller
{
    public static IServiceCollection AddBlogAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthMockConfiguration>(configuration.GetSection(AuthMockConfiguration.Key));

        var authMock = configuration
           .GetSection(AuthMockConfiguration.Key)
           .Get<AuthMockConfiguration>()!;

        if (authMock.Enabled)
        {
            services.Configure<AuthHandlerMock.AuthHandlerMockOptions>(_ => { });

            services
                .AddAuthentication(AuthHandlerMock.AuthenticationScheme)
                .AddScheme<AuthHandlerMock.AuthHandlerMockOptions, AuthHandlerMock>(AuthHandlerMock.AuthenticationScheme, _ => { });

            services.AddScoped<IApplicationUserProvider, MockApplicationUserProvider>();
        }
        else
        {
            services.AddAuthentication()
                .AddCookie(IdentityConstants.ApplicationScheme);

            services.AddScoped<IApplicationUserProvider, ApplicationUserProvider>();
        }

        services.AddAuthorization(x =>
        {
            x.AddPolicy(BlogAuthPolicies.AdminAccess, policy => policy.RequireRole(BlogRoles.Admin));
        });

        services
            .AddIdentityCore<User>(x =>
            {
                x.Password.RequireDigit = true;
                x.Password.RequiredLength = 8;
                x.Password.RequireLowercase = true;
                x.Password.RequireNonAlphanumeric = true;
                x.Password.RequireUppercase = true;

                x.User.RequireUniqueEmail = true;

                x.SignIn.RequireConfirmedEmail = true;

                x.Lockout.AllowedForNewUsers = true;
                x.Lockout.MaxFailedAccessAttempts = 5;
                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<BlogDbContext>()
            .AddApiEndpoints();

        return services;
    }
}