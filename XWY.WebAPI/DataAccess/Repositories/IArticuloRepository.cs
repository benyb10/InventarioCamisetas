using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Repositories
{
    public interface IArticuloRepository : IGenericRepository<Articulo>
    {
        Task<Articulo?> GetByCodigoAsync(string codigo);
        Task<Articulo?> GetByIdWithRelationsAsync(int id);
        Task<IEnumerable<Articulo>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<Articulo>> GetByEstadoAsync(int estadoId);
        Task<IEnumerable<Articulo>> GetByEquipoAsync(string equipo);
        Task<IEnumerable<Articulo>> GetAvailableAsync();
        Task<IEnumerable<Articulo>> GetWithRelationsAsync();
        Task<IEnumerable<Articulo>> SearchAsync(string searchTerm);
        Task<IEnumerable<Articulo>> GetFilteredAsync(int? categoriaId, int? estadoId, string? searchTerm);
        Task<IEnumerable<Articulo>> GetPagedWithFiltersAsync(int pageNumber, int pageSize, int? categoriaId, int? estadoId, string? searchTerm, string? orderBy, bool ascending = true);
        Task<bool> ExistsByCodigoAsync(string codigo);
        Task<bool> ExistsByCodigoAsync(string codigo, int excludeId);
        Task<bool> CanBeDeletedAsync(int id);
        Task<int> GetTotalStockAsync();
        Task<int> GetAvailableStockAsync();
        Task<IEnumerable<Articulo>> GetLowStockAsync(int minStock = 1);
    }
}
