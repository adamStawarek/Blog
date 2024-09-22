using Blog.Domain.Entities.Base;

namespace Blog.Domain.Entities;
public class User : EntityBase<User>
{
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    #region Relations

    public ICollection<Article> Articles { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = null!;

    #endregion
}