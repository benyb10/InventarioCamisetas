using Microsoft.EntityFrameworkCore.Storage;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.DataAccess.Repositories;

namespace XWY.WebAPI.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly XWYDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        private IUsuarioRepository? _usuarios;
        private IArticuloRepository? _articulos;
        private IPrestamoRepository? _prestamos;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(XWYDbContext context)
        {
            _context = context;
        }

        public IUsuarioRepository Usuarios
        {
            get
            {
                _usuarios ??= new UsuarioRepository(_context);
                return _usuarios;
            }
        }

        public IArticuloRepository Articulos
        {
            get
            {
                _articulos ??= new ArticuloRepository(_context);
                return _articulos;
            }
        }

        public IPrestamoRepository Prestamos
        {
            get
            {
                _prestamos ??= new PrestamoRepository(_context);
                return _prestamos;
            }
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (_repositories.ContainsKey(type))
                return (IGenericRepository<TEntity>)_repositories[type];

            var repository = new GenericRepository<TEntity>(_context);
            _repositories.Add(type, repository);

            return repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
