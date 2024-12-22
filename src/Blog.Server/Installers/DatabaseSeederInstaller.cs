using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Blog.Server.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Blog.Server.Extensions;
public static class DatabaseSeederInstaller
{
    public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        using var scope = app.ApplicationServices.CreateScope();

        var authMock = scope.ServiceProvider.GetRequiredService<IOptions<AuthMockConfiguration>>().Value;

        if (!authMock.Enabled || !authMock.SeedDatabase)
        {
            return app;
        }

        await EnsureDatabase(scope);

        await EnsureRoles(scope);

        await EnsureCurrentUser(scope);

        return app;
    }

    private static async Task EnsureDatabase(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

        await context.Database.EnsureCreatedAsync();
    }

    private static async Task EnsureRoles(IServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new("Admin"));
        }
    }

    private static async Task EnsureCurrentUser(IServiceScope scope)
    {
        var userProvider = scope.ServiceProvider.GetRequiredService<IOptions<AuthMockConfiguration>>();
        var user = userProvider.Value.User!;

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var existingUser = await userManager.FindByEmailAsync(user.Email);

        if (existingUser is not null)
        {
            return;
        }

        await userManager.CreateAsync(new User
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        }, user.Password);

        var createdUser = await userManager.FindByEmailAsync(user.Email);

        foreach (var role in user.Roles)
        {
            await userManager.AddToRoleAsync(createdUser!, role);
        }
    }
}