using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Blog.Server.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Blog.Server.Extensions;
public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        using var scope = app.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

        await context.Database.EnsureCreatedAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new("Admin"));
        }

        var userProvider = scope.ServiceProvider.GetRequiredService<IOptions<AuthMockConfiguration>>();
        var user = userProvider.Value.User!;

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var existingUser = await userManager.FindByEmailAsync(user.Email);

        if (existingUser is not null)
        {
            return app;
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

        return app;
    }
}