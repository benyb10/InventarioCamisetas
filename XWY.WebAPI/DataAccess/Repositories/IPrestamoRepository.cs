using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Repositories
{
    public interface IPrestamoRepository : IGenericRepository<Prestamo>
    {
        Task<Prestamo?> GetByIdWithRelationsAsync(int id);
        Task<IEnumerable<Prestamo>> GetWithRelationsAsync();
        Task<IEnumerable<Prestamo>> GetByUsuarioAsync(int usuarioId);
        Task<IEnumerable<Prestamo>> GetByArticuloAsync(int articuloId);
        Task<IEnumerable<Prestamo>> GetByEstadoAsync(int estadoId);
        Task<IEnumerable<Prestamo>> GetPendientesAsync();
        Task<IEnumerable<Prestamo>> GetAprobadosAsync();
        Task<IEnumerable<Prestamo>> GetEntregadosAsync();
        Task<IEnumerable<Prestamo>> GetVencidosAsync();
        Task<IEnumerable<Prestamo>> GetActivosAsync();
        Task<IEnumerable<Prestamo>> GetHistorialAsync();
        Task<IEnumerable<Prestamo>> GetByUsuarioAndEstadoAsync(int usuarioId, int estadoId);
        Task<IEnumerable<Prestamo>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Prestamo>> GetFilteredAsync(int? usuarioId, int? articuloId, int? estadoId, DateTime? fechaInicio, DateTime? fechaFin);
        Task<IEnumerable<Prestamo>> GetPagedWithFiltersAsync(int pageNumber, int pageSize, int? usuarioId, int? articuloId, int? estadoId, DateTime? fechaInicio, DateTime? fechaFin, string? orderBy, bool ascending = true);
        Task<bool> HasActiveLoanAsync(int articuloId);
        Task<bool> UserHasActiveLoanAsync(int usuarioId, int articuloId);
        Task<int> GetTotalPrestamosAsync();
        Task<int> GetPendientesCountAsync();
        Task<int> GetVencidosCountAsync();
        Task<Prestamo?> GetActiveByArticuloAsync(int articuloId);
    }
}
