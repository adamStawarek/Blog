namespace Blog.Domain.Interfaces;
public interface ISetId<in TId> where TId : IEquatable<TId>
{
    void SetId(TId id);
}