namespace Blog.Domain.Interfaces;
public interface IEntity
{
    IEntityId Id { get; }
}

public interface IEntity<TId> : IEntity where TId : IEquatable<TId>
{
    new IEntityId<TId> Id { get; }
}