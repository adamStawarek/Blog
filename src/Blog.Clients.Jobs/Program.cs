using Blog.Application;
using Blog.Application.Jobs.Base;
using Blog.Clients.Jobs.Jobs;
using Blog.Clients.Jobs.Utils;
using Blog.Infrastructure;
using Blog.Infrastructure.Database.Interceptors;
using Hangfire;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Jobs")
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

builder.Services
    .AddBlogDatabase(builder.Configuration)
    .AddBlogBackgroundServices(builder.Configuration)
    .AddBlogEmailSender(builder.Configuration)
    .AddBlogFileStorage(builder.Configuration)
    .AddBlogAppServices(builder.Configuration);

builder.Services.Configure<JobsOptions>(builder.Configuration.GetSection(JobsOptions.Key));
builder.Services.Configure<DatabaseBackupJobOptions>(builder.Configuration.GetSection(DatabaseBackupJobOptions.Key));

builder.Services.AddTransient(_ => new AuditContext("JOBS", DateTime.Now));

builder.Services.AddHangfireServer();

builder.Services.AddHostedService<JobsHostedService>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<JobsHostedService>()
    .AddClasses(classes => classes.AssignableTo<IBlogJob>())
    .AddClasses(classes => classes.AssignableTo(typeof(IBlogJob<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();