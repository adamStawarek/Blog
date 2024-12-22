using Blog.Domain.Entities;
using Blog.Server.Contracts;
using Blog.Tests.DatabaseUtils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Blog.Server.Tests.Integration.Features.Articles;
public class UpdateArticleTests : TestBase
{
    public UpdateArticleTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Retur200OkAndUpdateArticleInDatabase_When_EndpointIsCalled()
    {
        // Arrange
        BlogDbSeeder.Create(Context)
            .Add<Article>(x =>
            {
                x.Title = "Title";
                x.Description = "Description";
                x.Tags = new List<string> { "Tag" };
                x.Content = "Content";
                x.AuthorId = UserId;
            }, out var article);

        // Act
        var response = await Client.PutAsJsonAsync($"/api/articles/{article.Id.Value}", new EditArticleRequest
        {
            Title = "Title2",
            Content = "Content2",
            Description = "Description2",
            Tags = new List<string> { "Tag2" }
        });

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedArticle = await Context.Article.SingleOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.NotNull(updatedArticle);
            Assert.Equal("Title2", updatedArticle!.Title);
            Assert.Equal("Content2", updatedArticle.Content);
            Assert.Equal("Description2", updatedArticle.Description);
            Assert.Single(updatedArticle.Tags);
            Assert.Equal("Tag2", updatedArticle.Tags[0]);
        });
    }
}
