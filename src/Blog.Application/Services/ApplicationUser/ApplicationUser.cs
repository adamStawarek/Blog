using Blog.Domain.Roles;

namespace Blog.Application.Services.ApplicationUser;

public class ApplicationUser
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public ApplicationRole Role { get; set; }
}