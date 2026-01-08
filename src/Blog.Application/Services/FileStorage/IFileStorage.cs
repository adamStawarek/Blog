namespace Blog.Application.Services.FileStorage;

public interface IFileStorage
{
    Task<string> UploadFileAsync(string fileName, Stream content, string contentType);
}