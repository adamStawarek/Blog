namespace Blog.Application.Services.ApplicationUser;
public interface IApplicationUserProvider
{
    Task<ApplicationUser> GetAsync(CancellationToken cancellationToken = default);
}