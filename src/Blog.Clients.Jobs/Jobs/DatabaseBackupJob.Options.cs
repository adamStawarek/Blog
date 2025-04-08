namespace Blog.Clients.Web.Jobs.Jobs;
public sealed class DatabaseBackupJobOptions
{
    public const string Key = "DatabaseBackup";

    public required string BackupDirectory { get; set; }
}