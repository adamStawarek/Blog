namespace Blog.Server.Configurations;
public class AuthMockConfiguration
{
    public const string Key = "AuthMock";

    public bool Enabled { get; set; }
    public MockUser? User { get; set; }

    public class MockUser
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
