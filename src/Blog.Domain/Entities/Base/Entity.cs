using Blog.Domain.Interfaces;

namespace Blog.Domain.Entities.Base;
public abstract class Entity<TEntity, TId> : IEntity<TId>, IEquatable<Entity<TEntity, TId>>
    where TEntity : IEntity<TId>
    where TId : IEquatable<TId>
{
    protected Entity()
        : this(default(TId)!)
    {
    }

    protected Entity(TId id)
    {
        Id = new EntityId(id);
    }

    protected Entity(EntityId id)
    {
        Id = id;
    }

    public EntityId Id { get; private set; }

    IEntityId<TId> IEntity<TId>.Id => Id;

    IEntityId IEntity.Id => Id;

    public static bool operator ==(Entity<TEntity, TId>? left, Entity<TEntity, TId>? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<TEntity, TId>? left, Entity<TEntity, TId>? right) => !(left == right);

    public virtual bool Equals(Entity<TEntity, TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => obj is Entity<TEntity, TId> other && Equals(other);

    public override int GetHashCode() => GetType().GetHashCode() ^ Id.GetHashCode();

    public sealed record EntityId : IEntityId<TId>
    {
        private TId _value;

        public EntityId()
            : this(default(TId)!)
        {
        }

        public EntityId(TId value)
        {
            _value = value;
        }

        public TId Value => _value;

        object IEntityId.Value => _value;

        TId IEntityId<TId>.Value { get => _value; set => _value = value; }

        public static implicit operator TId(EntityId entityId)
        {
            return entityId.Value;
        }

        public override string ToString() => Value.ToString();
    }
}