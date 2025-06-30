using Microsoft.EntityFrameworkCore;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Repositories
{
    public class PrestamoRepository : GenericRepository<Prestamo>, IPrestamoRepository
    {
        public PrestamoRepository(XWYDbContext context) : base(context)
        {
        }

        public async Task<Prestamo?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.EstadoArticulo)
                .Include(p => p.EstadoPrestamo)
                .Include(p => p.UsuarioAprobador)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Prestamo>> GetWithRelationsAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.EstadoArticulo)
                .Include(p => p.EstadoPrestamo)
                .Include(p => p.UsuarioAprobador)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetByUsuarioAsync(int usuarioId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetByArticuloAsync(int articuloId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.ArticuloId == articuloId)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetByEstadoAsync(int estadoId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.EstadoPrestamoId == estadoId)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetPendientesAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.EstadoPrestamo.Nombre == "Pendiente")
                .OrderBy(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetAprobadosAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.EstadoPrestamo.Nombre == "Aprobado")
                .OrderBy(p => p.FechaAprobacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetEntregadosAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.EstadoPrestamo.Nombre == "Entregado")
                .OrderByDescending(p => p.FechaEntregaReal)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetVencidosAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.EstadoPrestamo.Nombre == "Entregado" &&
                           p.FechaDevolucionEstimada.HasValue &&
                           p.FechaDevolucionEstimada.Value < DateTime.Now &&
                           !p.FechaDevolucionReal.HasValue)
                .OrderBy(p => p.FechaDevolucionEstimada)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetActivosAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.EstadoPrestamo.Nombre == "Pendiente" ||
                           p.EstadoPrestamo.Nombre == "Aprobado" ||
                           p.EstadoPrestamo.Nombre == "Entregado")
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetHistorialAsync()
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Include(p => p.UsuarioAprobador)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetByUsuarioAndEstadoAsync(int usuarioId, int estadoId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.UsuarioId == usuarioId && p.EstadoPrestamoId == estadoId)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .Where(p => p.FechaSolicitud >= fechaInicio && p.FechaSolicitud <= fechaFin)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetFilteredAsync(int? usuarioId, int? articuloId, int? estadoId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var query = _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .AsQueryable();

            if (usuarioId.HasValue)
                query = query.Where(p => p.UsuarioId == usuarioId.Value);

            if (articuloId.HasValue)
                query = query.Where(p => p.ArticuloId == articuloId.Value);

            if (estadoId.HasValue)
                query = query.Where(p => p.EstadoPrestamoId == estadoId.Value);

            if (fechaInicio.HasValue)
                query = query.Where(p => p.FechaSolicitud >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(p => p.FechaSolicitud <= fechaFin.Value);

            return await query.OrderByDescending(p => p.FechaSolicitud).ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetPagedWithFiltersAsync(int pageNumber, int pageSize, int? usuarioId, int? articuloId, int? estadoId, DateTime? fechaInicio, DateTime? fechaFin, string? orderBy, bool ascending = true)
        {
            var query = _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(p => p.EstadoPrestamo)
                .AsQueryable();

            if (usuarioId.HasValue)
                query = query.Where(p => p.UsuarioId == usuarioId.Value);

            if (articuloId.HasValue)
                query = query.Where(p => p.ArticuloId == articuloId.Value);

            if (estadoId.HasValue)
                query = query.Where(p => p.EstadoPrestamoId == estadoId.Value);

            if (fechaInicio.HasValue)
                query = query.Where(p => p.FechaSolicitud >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(p => p.FechaSolicitud <= fechaFin.Value);

            query = orderBy?.ToLower() switch
            {
                "usuario" => ascending ? query.OrderBy(p => p.Usuario.Nombres) : query.OrderByDescending(p => p.Usuario.Nombres),
                "articulo" => ascending ? query.OrderBy(p => p.Articulo.Nombre) : query.OrderByDescending(p => p.Articulo.Nombre),
                "estado" => ascending ? query.OrderBy(p => p.EstadoPrestamo.Nombre) : query.OrderByDescending(p => p.EstadoPrestamo.Nombre),
                "fechaentrega" => ascending ? query.OrderBy(p => p.FechaEntregaEstimada) : query.OrderByDescending(p => p.FechaEntregaEstimada),
                _ => ascending ? query.OrderBy(p => p.FechaSolicitud) : query.OrderByDescending(p => p.FechaSolicitud)
            };

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> HasActiveLoanAsync(int articuloId)
        {
            return await _dbSet.AnyAsync(p => p.ArticuloId == articuloId &&
                                             (p.EstadoPrestamo.Nombre == "Pendiente" ||
                                              p.EstadoPrestamo.Nombre == "Aprobado" ||
                                              p.EstadoPrestamo.Nombre == "Entregado"));
        }

        public async Task<bool> UserHasActiveLoanAsync(int usuarioId, int articuloId)
        {
            return await _dbSet.AnyAsync(p => p.UsuarioId == usuarioId &&
                                             p.ArticuloId == articuloId &&
                                             (p.EstadoPrestamo.Nombre == "Pendiente" ||
                                              p.EstadoPrestamo.Nombre == "Aprobado" ||
                                              p.EstadoPrestamo.Nombre == "Entregado"));
        }

        public async Task<int> GetTotalPrestamosAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> GetPendientesCountAsync()
        {
            return await _dbSet.CountAsync(p => p.EstadoPrestamo.Nombre == "Pendiente");
        }

        public async Task<int> GetVencidosCountAsync()
        {
            return await _dbSet.CountAsync(p => p.EstadoPrestamo.Nombre == "Entregado" &&
                                              p.FechaDevolucionEstimada.HasValue &&
                                              p.FechaDevolucionEstimada.Value < DateTime.Now &&
                                              !p.FechaDevolucionReal.HasValue);
        }

        public async Task<Prestamo?> GetActiveByArticuloAsync(int articuloId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.EstadoPrestamo)
                .FirstOrDefaultAsync(p => p.ArticuloId == articuloId &&
                                         (p.EstadoPrestamo.Nombre == "Pendiente" ||
                                          p.EstadoPrestamo.Nombre == "Aprobado" ||
                                          p.EstadoPrestamo.Nombre == "Entregado"));
        }
    }
}
