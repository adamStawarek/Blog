namespace Blog.Domain.Entities.Base;
public interface ISoftDelete
{
    bool Meta_IsActive { get; }

    void Activate();
    void Deactivate();
}