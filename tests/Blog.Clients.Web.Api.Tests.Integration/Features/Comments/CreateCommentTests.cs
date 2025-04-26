using Blog.Clients.Web.Api.Contracts;
using Blog.Domain.Entities;
using Blog.Tests.DatabaseUtils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Blog.Clients.Web.Api.Tests.Integration.Features.Comments;
public class CreateCommentTests : TestBase
{
    public CreateCommentTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Retur200OkAndCreateCommentInDatabase_When_EndpointIsCalled()
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
        var response = await Client.PostAsJsonAsync($"/api/articles/{article.Id.Value}/comments", new CreateCommentRequest
        {
            Content = "Content"
        });

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var comment = await Context.Comment.SingleOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.NotNull(comment);
            Assert.Equal("Content", comment.Content);
            Assert.Equal(article.Id, comment.ArticleId);
        });
    }
}