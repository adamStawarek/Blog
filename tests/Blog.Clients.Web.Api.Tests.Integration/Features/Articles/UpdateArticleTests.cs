using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Blog.Domain.Entities.Enumerators;
using Blog.Tests.DatabaseUtils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Blog.Clients.Web.Api.Tests.Integration.Features.Articles;

public class UpdateArticleTests : TestBase
{
    public UpdateArticleTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Return200OkAndUpdateArticleInDatabase_When_EndpointIsCalled()
    {
        // Arrange
        BlogDbSeeder.Create(Context)
            .Add<Article>(x =>
            {
                x.Title = "Title";
                x.Description = "Description";
                x.Tags = ["Tag"];
                x.Content = "Content";
                x.AuthorId = UserId;
            }, out var article);

        // Act
        var response = await Client.PutAsJsonAsync($"/api/articles/{article.Id.Value}", new EditArticleRequest
        {
            Title = "Title2",
            Content = "Content2",
            Description = "Description2",
            Status = ArticleStatus.Draft,
            Tags = ["Tag2"]
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedArticle = await Context.Article
            .SingleOrDefaultAsync();

        await Verify(updatedArticle);
    }
}