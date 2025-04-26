namespace Blog.Domain.Entities.Base;
public interface ISetId<in TId> where TId : IEquatable<TId>
{
    void SetId(TId id);
}