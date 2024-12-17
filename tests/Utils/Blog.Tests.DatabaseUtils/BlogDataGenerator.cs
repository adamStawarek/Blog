using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using Bogus;

namespace Blog.Tests.DatabaseUtils;
internal static class BlogDataGenerator
{
    public static Action<T> Generate<T>() where T : IEntity => x =>
    {
        switch (x)
        {
            case User user:
                UserGenerator.Populate(user);
                return;
            case Article article:
                ArticleGenerator.Populate(article);
                return;
            case Comment comment:
                CommentGenerator.Populate(comment);
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    };

    private static readonly Faker<User> UserGenerator = new Faker<User>()
        .RuleFor(x => x.DisplayName, x => x.Person.FullName)
        .RuleFor(x => x.FirstName, x => x.Person.FirstName)
        .RuleFor(x => x.LastName, x => x.Person.LastName);

    private static readonly Faker<Article> ArticleGenerator = new Faker<Article>()
        .RuleFor(x => x.Title, x => x.Lorem.Sentence())
        .RuleFor(x => x.Tags, x => new List<string> { x.PickRandom("c#", "java", "sql") })
        .RuleFor(x => x.Content, x => x.Lorem.Paragraphs(5));

    private static readonly Faker<Comment> CommentGenerator = new Faker<Comment>()
        .RuleFor(x => x.Content, x => x.Lorem.Paragraphs(1));
}