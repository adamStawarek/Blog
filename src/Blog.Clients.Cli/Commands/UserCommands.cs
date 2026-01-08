using Blog.Domain.Entities;
using Cocona;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Clients.Cli.Commands;

public class UserCommands(IServiceProvider serviceProvider)
{
    [Command("add")]
    public async Task AddNewUserAsync([Option] string name, [Option] string email, [Option] string password)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        await userManager.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            UserName = name,
            Email = email,
            EmailConfirmed = true
        }, password);
    }

    [Command("assign-role")]
    public async Task AssignRoleToUser([Option] string user, [Option] string role)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var exsitingUser = await userManager.FindByNameAsync(user);

        await userManager.AddToRoleAsync(exsitingUser!, role);
    }
}