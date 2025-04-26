using Blog.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace Blog.Domain.Entities;
public class User : IdentityUser<Guid>, IEntity
{
    IEntityId IEntity.Id => new EntityIdBase<Guid>(Id);

    #region Relations

    public ICollection<Article> Articles { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = null!;

    #endregion
}