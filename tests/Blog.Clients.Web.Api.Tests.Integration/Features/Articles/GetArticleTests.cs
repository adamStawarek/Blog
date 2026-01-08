using Blog.Domain.Entities;
using Blog.Tests.DatabaseUtils;
using System.Net;

namespace Blog.Clients.Web.Api.Tests.Integration.Features.Articles;

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
                x.Tags = ["Tag"];
                x.Content = "Content";
                x.AuthorId = UserId;
            }, out var article)
            .Add<User>(x =>
            {
                x.Id = Guid.NewGuid();
                x.UserName = "Bobby";
            }, out var reader)
            .Add<Comment>(x =>
            {
                x.AuthorId = UserId;
                x.ArticleId = article.Id;
                x.Content = "Comment A";
                x.ChildComments =
                [
                    new Comment
                    {
                        AuthorId = reader.Id,
                        ArticleId = article.Id,
                        Content = "SubComment B"
                    }
                ];
            }, out Comment _);

        // Act
        var response = await Client.GetAsync($"/api/articles/{article.Id.Value}");

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await VerifyJson(responseBody);
    }
}