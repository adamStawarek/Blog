using Blog.Application.Services.Jobs;
using Blog.Clients.Web.Api.Tests.Integration.Mocks;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System.Data.Common;
using Testcontainers.MsSql;

namespace Blog.Clients.Web.Api.Tests.Integration;

public sealed class BlogApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;

    public HttpClient HttpClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbConnection = new SqlConnection(_container.GetConnectionString());

        HttpClient = CreateClient();

        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            CheckTemporalTables = true
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");

        builder.ConfigureServices(services =>
        {
            RemoveDbContext(services);
            AddDbContext(services, _container.GetConnectionString());
            EnsureDbCreated(services);

            services.RemoveAll<IBackgroundJobProcessor>();
            services.AddSingleton<IBackgroundJobProcessor, BackgroundJobProcessorMock>();
        });
    }

    private static void RemoveDbContext(IServiceCollection services)
    {
        var dbContextDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<BlogDbContext>));

        services.Remove(dbContextDescriptor!);

        var dbConnectionDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbConnection));

        services.Remove(dbConnectionDescriptor!);
    }

    private static void AddDbContext(IServiceCollection services, string connString)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseSqlServer(connString, x =>
                x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
    }

    private static void EnsureDbCreated(IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<BlogDbContext>();
        context.Database.EnsureCreated();
    }
}