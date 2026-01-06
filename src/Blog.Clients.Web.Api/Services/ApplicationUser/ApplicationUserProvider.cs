using Blog.Application.Services.ApplicationUser;
using Blog.Clients.Web.Api.Auth;
using Blog.Domain.Roles;
using System.Security.Claims;

namespace Blog.Clients.Web.Api.Services.ApplicationUser;
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
                UserName = "Anonymous",
                Role = ApplicationRole.AnonymousReader
            });
        }

        var userId = identity
            .FindFirst(x => x.Type == ClaimTypes.NameIdentifier)!
            .Value;

        var userName = identity
            .FindFirst(x => x.Type == ClaimTypes.Name)!
            .Value;

        var isAdmin = identity.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Any(x => x.Value == BlogRoles.Admin);

        var userRole = isAdmin ? 
            ApplicationRole.Administrator : 
            ApplicationRole.AuthenticatedReader;

        return Task.FromResult(new Application.Services.ApplicationUser.ApplicationUser
        {
            Id = Guid.Parse(userId),
            UserName = userName,
            Role = userRole
        });
    }
}