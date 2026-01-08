using Blog.Domain.Entities;
using Blog.Tests.DatabaseUtils;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Blog.Clients.Web.Api.Tests.Integration.Features.Articles;

public class DeleteArticleTests : TestBase
{
    public DeleteArticleTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Return200OkAndSoftDeleteArticleInDatabase_When_EndpointIsCalled()
    {
        // Arrange
        BlogDbSeeder.Create(Context)
            .Add<Article>(x =>
            {
                x.AuthorId = UserId;
            }, out var article);

        // Act
        var response = await Client.DeleteAsync($"/api/articles/{article.Id.Value}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedArticle = await Context.Article
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.NotNull(updatedArticle);
            Assert.False(updatedArticle.Meta_IsActive);
        });
    }
}