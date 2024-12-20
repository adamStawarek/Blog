using Blog.Application;
using Blog.Application.Services.ApplicationUser;
using Blog.Application.Services.CurrentTime;
using Blog.Domain.Entities;
using Blog.Infrastructure;
using Blog.Infrastructure.Database;
using Blog.Infrastructure.Database.Interceptors;
using Blog.Infrastructure.DatabaseMigrations;
using Blog.Server.Auth;
using Blog.Server.Errors;
using Blog.Server.Services.ApplicationUser;
using Blog.Server.Validation;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Blog.Server;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.Configure<AuthMockConfiguration>(
            builder.Configuration.GetSection(AuthMockConfiguration.Key));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var authMockConfiguration = builder.Configuration.GetSection(
            AuthMockConfiguration.Key).Get<AuthMockConfiguration>()!;

        if (authMockConfiguration.Enabled)
        {
            builder.Services.Configure<AuthHandlerMock.AuthHandlerMockOptions>(_ => { });

            builder.Services.AddAuthentication(AuthHandlerMock.AuthenticationScheme)
                .AddScheme<AuthHandlerMock.AuthHandlerMockOptions, AuthHandlerMock>(AuthHandlerMock.AuthenticationScheme, _ => { });
        }
        else
        {
            builder.Services.AddAuthentication()
                .AddCookie(IdentityConstants.ApplicationScheme);
        }

        builder.Services.AddAuthorization();

        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<BlogDbContext>()
            .AddApiEndpoints();

        builder.Services.AddScoped<IApplicationUserProvider, MockApplicationUserProvider>();

        builder.Services.AddTransient(serviceProvider =>
        {
            var currentTimeProvider = serviceProvider.GetRequiredService<ICurrentTimeProvider>();
            var currentUserProvider = serviceProvider.GetRequiredService<IApplicationUserProvider>();
            var currentUser = currentUserProvider.GetAsync().Result;
            return new AuditContext(currentUser.UserName, currentTimeProvider.Now().DateTime);
        });

        builder.Services.AddBlogServices(builder.Configuration);

        builder.Services.AddBlogInfrastructure(builder.Configuration, options =>
            options.AllowMigrationManagement());

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddCarter();

        builder.Services.AddOpenApiDocument(settings =>
        {
            settings.PostProcess = document =>
            {
                document.Info.Version = "v1";
                document.Info.Title = "Blog API";
            };
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            //TODO: Disabled to run tests
            //app.SeedDatabase();

            app.UseOpenApi();
            app.UseSwaggerUI();
        }

        app.MapCarter();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapIdentityApi<User>();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}