namespace Blog.Clients.Jobs.Utils;
public class JobsOptions
{
    public const string Key = "Hangfire";

    public List<Configuration> Jobs { get; set; } = new();

    public class Configuration
    {
        public string JobId { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string Cron { get; set; } = string.Empty;
    }
}