using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Blog.Clients.Web.Api.Features.Account;
public class GetAccountInfoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapGet("account/info", (IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) =>
        {
            var identity = httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;

            return Results.Ok(new GetAccountInfoResponse
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
        .WithName("GetAccountInfo")
        .Produces<GetAccountInfoResponse>();
}