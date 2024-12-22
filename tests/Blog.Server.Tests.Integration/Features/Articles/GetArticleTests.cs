using Blog.Domain.Entities;
using Blog.Tests.DatabaseUtils;
using System.Net;

namespace Blog.Server.Tests.Integration.Features.Articles;
public class GetArticleTests : TestBase
{
    public GetArticleTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnArticle_When_EndpointIsCalled()
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
        var response = await Client.GetAsync($"/api/articles/{article.Id.Value}");

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await VerifyJson(responseBody);
    }
}