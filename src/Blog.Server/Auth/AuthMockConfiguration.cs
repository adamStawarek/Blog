namespace Blog.Server.Auth;
public class AuthMockConfiguration
{
    public const string Key = "AuthMock";

    public bool Enabled { get; set; }
    public bool SeedDatabase { get; set; }
    public MockUser? User { get; set; }

    public class MockUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}