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
    public static void Main(string[] args)
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

        // Redirect HTTP to HTTPS
        app.UseHttpsRedirection();

        // Serve static files (images, CSS, JS)
        app.UseStaticFiles();

        // Serve default files (like index.html)
        app.UseDefaultFiles();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi(x =>
            {
                x.PostProcess = (document, _) =>
                {
                    document.Info.Title = "Blog API";
                    document.Info.Version = "v1";
                };
            });

            app.UseReDoc(x =>
            {
                x.Path = "/docs";
            });
        }

        app.MapCarter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/account")
           .WithTags("Account")
           .MapIdentityApi<User>();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}