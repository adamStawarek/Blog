using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities.Enumerators;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Blog.Clients.Web.Api.Tests.Integration.Features.Articles;
public class CreateArticleTests : TestBase
{
    public CreateArticleTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Return200OkAndCreateArticleInDatabase_When_EndpointIsCalled()
    {
        // Act
        var response = await Client.PostAsJsonAsync($"/api/articles", new CreateArticleRequest
        {
            Title = "Title",
            Content = "Content",
            Description = "Description",
            Status = ArticleStatus.Ready,
            Tags = ["Tag"]
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var article = await Context.Article
            .SingleOrDefaultAsync();

        await Verify(article);
    }
}