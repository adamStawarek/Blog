using Blog.Domain.Entities;
using Blog.Infrastructure.Database;
using Blog.Server.Auth;
using Blog.Tests.DatabaseUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Blog.Server.Tests.Integration;

[Collection("Default")]
public abstract class TestBase : IAsyncLifetime
{
    protected readonly BlogApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected IServiceScope ServiceScope = default!;
    protected BlogDbContext Context = default!;
    protected Guid UserId = default!;

    protected TestBase(BlogApplicationFactory factory)
    {
        Factory = factory;
        Client = Factory.HttpClient;
    }

    protected virtual void InitializeCurrentUserData()
    {
        var authMock = ServiceScope.ServiceProvider.GetService<IOptions<AuthMockConfiguration>>()!;

        BlogDbSeeder.Create(Context)
            .Add<User>(x =>
            {
                x.Id = authMock.Value.User!.Id;
                x.UserName = authMock.Value.User!.UserName;
            }, out var user);

        UserId = user.Id;
    }

    public Task InitializeAsync()
    {
        ServiceScope = Factory.Services.CreateScope();
        Context = ServiceScope.ServiceProvider.GetRequiredService<BlogDbContext>();

        InitializeCurrentUserData();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }
}