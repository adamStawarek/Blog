using Blog.Application.Services.FileStorage;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Options;

namespace Blog.Infrastructure.FileStorage;
public sealed class GoogleDriveFileStorage : IFileStorage
{
    private readonly GoogleDriveFileStorageOptions _options;

    private DriveService _driveService = null!;
    private DriveService DriveService => _driveService ??= CreateGoogleDriveService();

    public GoogleDriveFileStorage(IOptions<GoogleDriveFileStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream content, string contentType)
    {
        var driveFile = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName,
            Parents = [_options.RootDirectoryId],
            MimeType = contentType
        };

        var request = DriveService.Files.Create(driveFile, content, contentType);
        request.Fields = "id";

        var file = await request.UploadAsync();

        if (file.Status != Google.Apis.Upload.UploadStatus.Completed)
        {
            throw new Exception($"File upload failed: {file.Exception}");
        }

        return request.ResponseBody.Id;
    }

    private DriveService CreateGoogleDriveService()
    {
        var credential = GoogleCredential.FromJson(_options.CredentialsJSON).CreateScoped(DriveService.ScopeConstants.Drive);

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _options.ApplicationName,
        });
    }
}