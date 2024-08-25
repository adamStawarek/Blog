using Blog.Application;
using Blog.Application.Services.ApplicationUser;
using Blog.Application.Services.CurrentTime;
using Blog.Infrastructure;
using Blog.Infrastructure.Database.Interceptors;
using Blog.Infrastructure.DatabaseMigrations;
using Blog.Server.Auth;
using Blog.Server.Configurations;
using Blog.Server.Services.ApplicationUser;
using Carter;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthMockConfiguration>(builder.Configuration.GetSection(AuthMockConfiguration.Key));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authMockConfiguration = builder.Configuration.GetSection(AuthMockConfiguration.Key).Get<AuthMockConfiguration>()!;

if (authMockConfiguration.Enabled)
{
    builder.Services.Configure<AuthHandlerMock.AuthHandlerMockOptions>(_ => { });
    builder.Services
        .AddAuthentication(AuthHandlerMock.AuthenticationScheme)
        .AddScheme<AuthHandlerMock.AuthHandlerMockOptions, AuthHandlerMock>(AuthHandlerMock.AuthenticationScheme, _ => { });
}

builder.Services.AddAuthorization();

builder.Services.AddScoped<IApplicationUserProvider, MockApplicationUserProvider>();

builder.Services.AddScoped(async serviceProvider =>
{
    var currentTimeProvider = serviceProvider.GetRequiredService<ICurrentTimeProvider>();
    var currentUserProvider = serviceProvider.GetRequiredService<IApplicationUserProvider>();
    var currentUser = await currentUserProvider.GetAsync(CancellationToken.None);
    return new AuditContext(currentUser.DisplayName, currentTimeProvider.Now.DateTime);
});

builder.Services.AddBlogServices(builder.Configuration);

builder.Services.AddBlogInfrastructure(builder.Configuration, options =>
    options.AllowMigrationManagement());

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddCarter();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToFile("/index.html");

app.Run();
