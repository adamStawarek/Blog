using Blog.Domain.Entities;

namespace Blog.Application.Services.ApplicationUser;
public class ApplicationUser
{
    public User.EntityId Id { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
}