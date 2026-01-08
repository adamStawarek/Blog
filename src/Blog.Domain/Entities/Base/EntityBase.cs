namespace Blog.Domain.Entities.Base;

public abstract class EntityBase<T> : Entity<T, Guid>, ISoftDelete, IAudit where T : IEntity<Guid>
{
    protected EntityBase() : base(Guid.NewGuid())
    {
    }

    protected EntityBase(Guid id) : base(id)
    {
    }

    public bool Meta_IsActive { get; private set; }
    public string Meta_CreatedBy { get; private set; } = null!;
    public DateTimeOffset Meta_CreatedDate { get; private set; }
    public string Meta_LastModifiedBy { get; private set; } = null!;
    public DateTimeOffset Meta_LastModifiedDate { get; private set; }

    void IAudit.SetCreated(string user, DateTimeOffset date)
    {
        Meta_CreatedBy = user;
        Meta_CreatedDate = date;
    }

    void IAudit.SetUpdated(string user, DateTimeOffset date)
    {
        Meta_LastModifiedBy = user;
        Meta_LastModifiedDate = date;
    }

    void ISoftDelete.Activate() => Meta_IsActive = true;

    void ISoftDelete.Deactivate() => Meta_IsActive = false;
}