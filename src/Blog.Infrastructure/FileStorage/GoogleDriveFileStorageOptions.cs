namespace Blog.Infrastructure.FileStorage;
public class GoogleDriveFileStorageOptions
{
    public const string Key = "GoogleDrive";

    public required string ApplicationName { get; set; }
    public required string RootDirectoryId { get; set; }
    public required string CredentialsJSON { get; set; }
}