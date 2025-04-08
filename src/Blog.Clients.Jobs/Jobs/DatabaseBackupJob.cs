using Blog.Application.Services.FileStorage;
using Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Blog.Clients.Web.Jobs.Jobs;
public class DatabaseBackupJob : IDatabaseBackupJob
{
    private readonly BlogDbContext _context;
    private readonly IFileStorage _fileStorage;

    public DatabaseBackupJob(BlogDbContext context, IFileStorage storage)
    {
        _context = context;
        _fileStorage = storage;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var backupFile = $"BlogDb_{timestamp}.bak";

        var backupDirectory = _context.Database
            .SqlQueryRaw<string>("DECLARE @path NVARCHAR(512); EXEC master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', " +
                "N'SOFTWARE\\Microsoft\\MSSQLServer\\MSSQLServer', " +
                "N'BackupDirectory', " +
                "@path OUTPUT; SELECT @path;")
            .AsEnumerable()
            .Single();

        var backupPath = Path.Combine(backupDirectory, backupFile);

        await _context.Database
          .ExecuteSqlAsync($"BACKUP DATABASE [BlogDb] TO DISK={backupPath}", cancellationToken);

        using var stream = new FileStream(backupPath, FileMode.Open, FileAccess.Read);
        await _fileStorage.UploadFileAsync(backupFile, stream, "application/octet-stream");
    }
}