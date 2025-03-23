using Blog.Application.Services.ApplicationUser;
using System.Security.Claims;

namespace Blog.Server.Services.ApplicationUser;
internal class ApplicationUserProvider : IApplicationUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Application.Services.ApplicationUser.ApplicationUser> GetAsync(CancellationToken cancellationToken = default)
    {
        var identity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;

        if (!identity!.IsAuthenticated)
        {
            return Task.FromResult(new Application.Services.ApplicationUser.ApplicationUser
            {
                Id = Guid.Empty,
                UserName = "System"
            });
        }

        return Task.FromResult(new Application.Services.ApplicationUser.ApplicationUser
        {
            Id = Guid.Parse(identity!.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)!.Value),
            UserName = identity!.FindFirst(x => x.Type == ClaimTypes.Name)!.Value
        });
    }
}