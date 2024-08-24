using Blog.Infrastructure;
using Blog.Infrastructure.DatabaseMigrations;
using Blog.Server.Features.Articles;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBlogInfrastructure(builder.Configuration, options => 
    options.AllowMigrationManagement());

builder.Services.AddMediatR(config => 
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

CreateArticle.MapEndpoints(app);

app.MapFallbackToFile("/index.html");

app.Run();
