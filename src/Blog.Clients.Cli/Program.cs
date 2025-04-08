using Blog.Cli.Commands;
using Blog.Domain.Entities;
using Blog.Infrastructure;
using Blog.Infrastructure.Database;
using Blog.Infrastructure.Database.Interceptors;
using Blog.Infrastructure.DatabaseMigrations;
using Cocona;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder();

builder.Configuration
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables();

builder.Logging.AddFilter("System.Net.Http", LogLevel.Warning);

builder.Services.AddBlogDatabase(builder.Configuration, x => x.AllowMigrationManagement());

builder.Services.AddTransient(_ => new AuditContext("CLI", DateTime.Now));

builder.Services
  .AddIdentityCore<User>()
  .AddRoles<IdentityRole<Guid>>()
  .AddEntityFrameworkStores<BlogDbContext>();

var app = builder.Build();

app.AddSubCommand("database", x => x.AddCommands<DatabaseCommands>());
app.AddSubCommand("users", x => x.AddCommands<UserCommands>());

app.Run();