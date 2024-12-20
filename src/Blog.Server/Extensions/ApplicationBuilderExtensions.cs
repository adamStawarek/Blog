using Blog.Application.Services.ApplicationUser;
using Blog.Domain.Entities;
using Blog.Infrastructure.Database;

namespace Blog.Server.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        using var scope = app.ApplicationServices.CreateScope();
        var userProvider = scope.ServiceProvider.GetRequiredService<IApplicationUserProvider>();

        var user = userProvider.GetAsync()
            .GetAwaiter()
            .GetResult();

        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

        context.Database.EnsureCreated();

        var usersExists = context.User.Any();
        if (usersExists)
        {
            return app;
        }

        var dbUser = new User
        {
            Id = user.Id,
            UserName = user.UserName,
            Articles = new List<Article>
                {
                    new Article
                    {
                        Title = "Dummy article",
                        Description = "Some dummy description",
                        Tags = new List<string>
                        {
                            "ABC", "BCD"
                        },
                        Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut sapien neque, placerat vitae sodales sit amet, blandit sit amet magna. " +
                        "Maecenas feugiat diam lorem, auctor tempor nulla dignissim et. Sed mollis ultricies mauris, vel consectetur lectus. " +
                        "Interdum et malesuada fames ac ante ipsum primis in faucibus. Curabitur sagittis commodo ante, id aliquam lectus ultricies eget. " +
                        "Mauris hendrerit, arcu ut volutpat vulputate, dolor neque suscipit felis, vitae aliquam arcu mauris sit amet diam. " +
                        "Vestibulum blandit sem eget auctor dapibus. Fusce ac dapibus nisl. Vestibulum bibendum congue finibus. Quisque quis auctor tellus. " +
                        "Fusce mattis lacinia ante eu pharetra. Vivamus magna tellus, sagittis vel lacus ac, consequat auctor justo. Cras id posuere elit, vitae vehicula ex. " +
                        "Mauris fermentum gravida nulla, id feugiat tortor vehicula quis."
                    }
                }
        };

        context.Add(dbUser);

        context.SaveChanges();

        return app;
    }
}