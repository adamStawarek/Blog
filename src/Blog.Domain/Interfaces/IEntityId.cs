namespace Blog.Domain.Interfaces;
public interface IEntityId<TId> : IEntityId where TId : IEquatable<TId>
{
    new TId Value { get; set; }
}

public interface IEntityId
{
    object Value { get; }
}