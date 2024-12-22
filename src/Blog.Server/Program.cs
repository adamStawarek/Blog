using Blog.Application;
using Blog.Domain.Entities;
using Blog.Infrastructure;
using Blog.Infrastructure.DatabaseMigrations;
using Blog.Server.Errors;
using Blog.Server.Extensions;
using Carter;
using Serilog;

namespace Blog.Server;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .AddBlogAuth(builder.Configuration)
            .AddBlogInfrastructure(builder.Configuration, x => x.AllowMigrationManagement())
            .AddBlogAppServices(builder.Configuration)
            .AddBlogWebServices(builder.Configuration);

        builder.Services.AddCarter();

        builder.Services.AddOpenApiDocument();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            await app.SeedDatabaseAsync();

            app.UseOpenApi();
            app.UseReDoc(c =>
            {
                c.DocumentTitle = "API Docs";
                c.Path = "/docs";
            });
        }

        app.MapCarter();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/api/account")
           .WithTags("Account")
           .MapIdentityApi<User>();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}