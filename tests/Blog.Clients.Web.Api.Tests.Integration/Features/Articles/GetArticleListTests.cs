using Blog.Domain.Entities;
using Blog.Tests.DatabaseUtils;
using System.Net;

namespace Blog.Clients.Web.Api.Tests.Integration.Features.Articles;

public class GetArticleListTests : TestBase
{
    public GetArticleListTests(BlogApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnArticleList_When_EndpointIsCalled()
    {
        // Arrange
        BlogDbSeeder.Create(Context)
            .Add<Article>(x =>
            {
                x.Title = "A";
                x.Description = "Description A";
                x.Tags = ["Tag1", "Tag2"];
                x.AuthorId = UserId;
            }, out _)
            .Add<Article>(x =>
            {
                x.Title = "B";
                x.Description = "Description B";
                x.Tags = ["Tag1"];
                x.AuthorId = UserId;
            }, out _)
            .Add<Article>(x =>
            {
                x.Title = "C";
                x.Description = "Description C";
                x.Tags = ["Tag2"];
                x.AuthorId = UserId;
            }, out _);

        // Act
        var response = await Client.GetAsync("/api/articles?page=0&itemsPerPage=5");

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await VerifyJson(responseBody);
    }
}