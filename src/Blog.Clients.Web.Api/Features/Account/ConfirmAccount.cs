using Blog.Application.Options;
using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Carter;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Blog.Clients.Web.Api.Features.Account;

public class ConfirmAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
       .MapGet("account/confirm", async ([AsParameters] ConfirmAccountRequest request, UserManager<User> userManager, IOptions<BlogGeneralOptions> options) =>
       {
           var user = await userManager.FindByIdAsync(request.UserId.ToString());

           if (user is null)
           {
               return Results.NotFound("User not found.");
           }

           var result = await userManager.ConfirmEmailAsync(user, request.Code);

           if (!result.Succeeded)
           {
               return Results.BadRequest(result.Errors);
           }

           return Results.Ok("Account confirmed.");
       })
       .WithTags("Account")
       .WithName("ConfirmAccount");
}