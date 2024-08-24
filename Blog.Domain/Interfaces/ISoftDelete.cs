namespace Blog.Domain.Interfaces;
public interface ISoftDelete
{
    bool Meta_IsActive { get; }

    void Activate();
    void Deactivate();
}