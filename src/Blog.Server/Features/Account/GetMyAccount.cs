using Blog.Domain.Entities;
using Blog.Server.Contracts;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Blog.Server.Features.Account;
public class GetMyAccountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapGet("api/account/info", async (IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) =>
        {
            var identity = httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;
            return Results.Ok(new GetMyAccountResponse
            {
                Id = Guid.Parse(identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                UserName = identity.FindFirst(ClaimTypes.Name)!.Value,
                Roles = identity
                    .FindAll(ClaimTypes.Role)
                    .Select(x => x.Value)
                    .ToList()
            });
        })
        .RequireAuthorization()
        .WithTags("Account")
        .WithName("GetMyAccount")
        .Produces<GetArticleResponse>();
}