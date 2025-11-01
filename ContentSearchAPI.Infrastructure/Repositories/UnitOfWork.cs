using ContentSearchAPI.Application.Interfaces.Repositories;
using ContentSearchAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContentSearchAPI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private IContentRepository? _contentRepository;
    private IProviderConfigRepository? _providerConfigRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IContentRepository Contents => _contentRepository ??= new ContentRepository(_context);

    public IProviderConfigRepository ProviderConfigs => _providerConfigRepository ??= new ProviderConfigRepository(_context);

    public IRepository<T> Repository<T>() where T : class
    {
        return new Repository<T>(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
