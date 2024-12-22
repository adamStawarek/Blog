using Blog.Server.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Blog.Server.Tests.Integration.Features.Articles;
public class CreateArticleTests : TestBase
{
    public CreateArticleTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Retur200OkAndCreateArticleInDatabase_When_EndpointIsCalled()
    {
        // Act
        var response = await Client.PostAsJsonAsync($"/api/articles", new CreateArticleRequest
        {
            Title = "Title",
            Content = "Content",
            Description = "Description",
            Tags = new List<string> { "Tag" }
        });

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var article = await Context.Article.SingleOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.NotNull(article);
            Assert.Equal("Title", article!.Title);
            Assert.Equal("Content", article.Content);
            Assert.Equal("Description", article.Description);
            Assert.Single(article.Tags);
            Assert.Equal("Tag", article.Tags[0]);
        });
    }
}
