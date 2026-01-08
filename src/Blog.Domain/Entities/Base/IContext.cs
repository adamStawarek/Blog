namespace Blog.Domain.Entities.Base;

public interface IContext
{
    IQueryable<TEntity> Get<TEntity>() where TEntity : class, IEntity;
    Task<IEntity> Get<TEntity>(IEntityId id, CancellationToken cancellationToken = default) where TEntity : class, IEntity;
    void Add(params IEntity[] entities);
    void Update(params IEntity[] entities);
    void Remove(params IEntity[] entities);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}