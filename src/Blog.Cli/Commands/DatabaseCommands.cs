using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Bogus;
using Cocona;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Cli.Commands;
public class DatabaseCommands(IServiceProvider serviceProvider)
{
    [Command("apply-migrations")]
    public async Task EnsureDatabaseAsync()
    {
        var context = serviceProvider.GetRequiredService<BlogDbContext>();

        await context.Database.MigrateAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new("Admin"));
        }
    }

    [Command("seed-data")]
    public async Task SeedDatabaseAsync([Option] string author)
    {
        var context = serviceProvider.GetRequiredService<BlogDbContext>();

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var exsitingUser = await userManager.FindByNameAsync(author);

        var articles = new Faker<Article>()
            .RuleFor(x => x.Title, x => $"Review: {x.Commerce.ProductName()}")
            .RuleFor(x => x.Content, x => x.Rant.Review(x.Commerce.ProductName()))
            .RuleFor(x => x.Description, x => x.Lorem.Paragraph())
            .RuleFor(x => x.Tags, x => ["Review"])
            .RuleFor(x => x.AuthorId, x => exsitingUser!.Id)
            .RuleFor(x => x.Id, x => new Article.EntityId(Guid.NewGuid()))
            .Generate(20);

        await context.Article.AddRangeAsync(articles);

        await context.SaveChangesAsync();
    }
}

