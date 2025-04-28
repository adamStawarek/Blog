using Blog.Application.Options;
using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Blog.Clients.Web.Api.Features.Account;
public class CreateAccountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app
       .MapPost("account", async (CreateAccountRequest request, UserManager<User> userManager, IEmailSender emailSender, IOptions<BlogGeneralOptions> options) =>
       {
           var user = new User
           {
               Id = Guid.NewGuid(),
               UserName = request.UserName,
               Email = request.Email,
               EmailConfirmed = false
           };

           var result = await userManager.CreateAsync(user, request.Password);

           if (!result.Succeeded)
           {
               return Results.BadRequest(result.Errors);
           }

           var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
           var domain = options.Value.Domain;
           var encodedToken = Uri.EscapeDataString(token);
           var confirmationLink = $"https://{domain}/Account/Confirm?userId={user.Id}&code={encodedToken}";

           await emailSender.SendEmailAsync(user.Email, "Confirm your email",
               $"Please confirm your email by clicking on the link: <a href='{confirmationLink}'>Confirm Email</a>");

           return Results.Ok("Account created. Please check your email to confirm your account.");
       })
       .WithTags("Account")
       .WithName("CreateAccount");
}
