using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace Blog.Infrastructure.DatabaseMigrations;
public static class ServiceCollectionExtensions
{
    public static SqlServerDbContextOptionsBuilder AllowMigrationManagement(this SqlServerDbContextOptionsBuilder sqlBuilder)
        => sqlBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
}