namespace Blog.Infrastructure.FileStorage;

public class GoogleDriveFileStorageOptions
{
    public const string Key = "GoogleDrive";

    public string ApplicationName { get; set; }
    public string RootDirectoryId { get; set; }
    public string CredentialsJSON { get; set; }
}