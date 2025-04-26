namespace Blog.Domain.Entities.Base;
public record EntityIdBase<TId> : IEntityId<TId> where TId : IEquatable<TId>
{
    private TId _value;
    public EntityIdBase() : this(default(TId)!)
    {
    }

    public EntityIdBase(TId value)
    {
        _value = value;
    }

    public TId Value => _value;

    object IEntityId.Value => _value;
    TId IEntityId<TId>.Value { get => _value; set => _value = value; }

    public static implicit operator TId(EntityIdBase<TId> entityId)
    {
        return entityId.Value;
    }

    public override string ToString() => Value.ToString()!;
}