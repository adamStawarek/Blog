using Blog.Application.Services.ApplicationUser;
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
            Id = _configuration.User!.Id,
            UserName = _configuration.User.UserName
        });
    }
}