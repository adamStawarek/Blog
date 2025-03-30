using Blog.Application;
using Blog.Application.Events.Base;
using Blog.Clients.Jobs.Utils;
using Blog.Clients.Web.Jobs.Jobs;
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
    .AddBlogInfrastructure(builder.Configuration)
    .AddBlogAppServices(builder.Configuration);

builder.Services.Configure<JobsOptions>(builder.Configuration.GetSection("Hangfire"));

builder.Services.AddTransient(_ => new AuditContext("JOBS", DateTime.Now));

builder.Services.AddHangfire(x =>
{
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireDbContext"));
});

builder.Services.AddHangfireServer();

builder.Services.AddHostedService<JobsHostedService>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<JobsHostedService>()
    .AddClasses(classes => classes.AssignableTo<IBlogJob>())
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
