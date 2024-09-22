using Blog.Server.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Blog.Server.Auth;
public sealed class AuthHandlerMock : AuthenticationHandler<AuthHandlerMock.AuthHandlerMockOptions>
{
    public const string AuthenticationScheme = "Mock";

    private readonly AuthMockConfiguration _configuration;
    
    public AuthHandlerMock(
        IOptionsMonitor<AuthMockConfiguration> configuration,
        IOptionsMonitor<AuthHandlerMockOptions> options,
        ILoggerFactory logger, 
        UrlEncoder encoder) : base(options, logger, encoder)
    {
        _configuration = configuration.CurrentValue;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = _configuration.User!;

        var claims = new List<Claim>
        {
            new(BlogClaimDefaults.Id, user.Id.ToString()),
            new(BlogClaimDefaults.DisplayName,user.DisplayName)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    public class AuthHandlerMockOptions : AuthenticationSchemeOptions
    {
    }
}

