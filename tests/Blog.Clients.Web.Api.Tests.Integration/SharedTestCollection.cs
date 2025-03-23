namespace Blog.Server.Tests.Integration;
[CollectionDefinition("Default", DisableParallelization = true)]
public class SharedTestCollection : ICollectionFixture<BlogApplicationFactory>
{
}