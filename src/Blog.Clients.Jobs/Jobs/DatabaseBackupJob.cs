using Blog.Application.Jobs;
using Blog.Application.Services.FileStorage;
using Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Blog.Clients.Jobs.Jobs;

public class DatabaseBackupJob : IDatabaseBackupJob
{
    private readonly BlogDbContext _context;
    private readonly DatabaseBackupJobOptions _options;
    private readonly IFileStorage _fileStorage;

    public DatabaseBackupJob(
        BlogDbContext context,
        IOptions<DatabaseBackupJobOptions> options,
        IFileStorage fileStorage)
    {
        _context = context;
        _options = options.Value;
        _fileStorage = fileStorage;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var backupFile = $"BlogDb_{timestamp}.bak";

        // Ensure the backup directory exists both on sql server file system and jobs file system
        var backupPath = Path.Combine(_options.BackupDirectory, backupFile);

        await _context.Database
          .ExecuteSqlAsync($"BACKUP DATABASE [BlogDb] TO DISK={backupPath}", cancellationToken);

        using var stream = new FileStream(backupPath, FileMode.Open, FileAccess.Read);
        await _fileStorage.UploadFileAsync(backupFile, stream, "application/octet-stream");
    }
}