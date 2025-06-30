using XWY.WebAPI.DataAccess.Repositories;

namespace XWY.WebAPI.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; } // Add this property to define "Usuarios"  
        IArticuloRepository Articulos { get; }
        IPrestamoRepository Prestamos { get; }
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
        int SaveChanges();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Dispose();
    }
}
