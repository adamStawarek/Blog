using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Blog.Clients.Web.Api.Features.Account;
public class CreateAccountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
        .MapPost("account", async (CreateAccountRequest request, UserManager<User> userManager) =>
        {
            await userManager.CreateAsync(new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = false
            }, request.Password);
        })
        .WithTags("Account")
        .WithName("CreateAccount");
}