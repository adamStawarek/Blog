using Blog.Application.Services.ApplicationUser;
using Blog.Domain.Entities;
using Blog.Domain.Entities.Base;
using Blog.Server.Auth;
using Microsoft.Extensions.Options;

namespace Blog.Server.Services.ApplicationUser;
internal class MockApplicationUserProvider : IApplicationUserProvider
{
    private readonly AuthMockConfiguration _configuration;

    public MockApplicationUserProvider(IOptionsMonitor<AuthMockConfiguration> configuration)
    {
        _configuration = configuration.CurrentValue;
    }

    public Task<Application.Services.ApplicationUser.ApplicationUser> GetAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Application.Services.ApplicationUser.ApplicationUser
        {
            Id = new Entity<User, Guid>.EntityId(_configuration.User!.Id),
            DisplayName = _configuration.User.DisplayName
        });
    }
}