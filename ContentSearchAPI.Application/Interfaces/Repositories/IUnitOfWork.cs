namespace ContentSearchAPI.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IContentRepository Contents { get; }
    IProviderConfigRepository ProviderConfigs { get; }
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
