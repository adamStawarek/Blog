using Blog.Application;
using Blog.Domain.Entities;
using Blog.Infrastructure;
using Blog.Infrastructure.DatabaseMigrations;
using Blog.Server.Auth;
using Blog.Server.Errors;
using Blog.Server.Extensions;
using Carter;
using Hangfire;
using Serilog;

namespace Blog.Server;
public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.Enrich.WithProperty("Application", "Web")
			.Enrich.WithProperty("MachineName", Environment.MachineName)
			.Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
			.WriteTo.File("/app/logs/log.txt", rollingInterval: RollingInterval.Day)
			.WriteTo.OpenTelemetry(x =>
			{
				x.Endpoint = $"{builder.Configuration["Seq:Url"]!}";
				x.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf;
				x.Headers = new Dictionary<string, string>
				{
					["x-seq-api-key"] = builder.Configuration["Seq:ApiKey"]!
				};
			})
			.ReadFrom.Configuration(builder.Configuration)
			.CreateLogger();

		builder.Services.AddSerilog();

		builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
		builder.Services.AddProblemDetails();

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddHangfire(x =>
		{
			x.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireDbContext"));
		});

		builder.Services
			.AddBlogAuth(builder.Configuration)
			.AddBlogInfrastructure(builder.Configuration, x => x.AllowMigrationManagement())
			.AddBlogAppServices(builder.Configuration)
			.AddBlogWebServices(builder.Configuration);

		builder.Services.AddCarter();

		builder.Services.AddOpenApiDocument();

		var app = builder.Build();

		app.UseHttpsRedirection();

		app.UseStaticFiles();

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

		app.MapHangfireDashboardWithAuthorizationPolicy(BlogAuthPolicies.AdminAccess);

		app.MapGroup("/account")
		   .WithTags("Account")
		   .MapIdentityApi<User>();

		app.MapFallbackToFile("/index.html");

		app.Run();
	}
}