using Microsoft.EntityFrameworkCore;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Repositories
{
    public class ArticuloRepository : GenericRepository<Articulo>, IArticuloRepository
    {
        public ArticuloRepository(XWYDbContext context) : base(context)
        {
        }

        public async Task<Articulo?> GetByCodigoAsync(string codigo)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .FirstOrDefaultAsync(a => a.Codigo == codigo && a.Activo);
        }

        public async Task<Articulo?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);
        }

        public async Task<IEnumerable<Articulo>> GetByCategoriaAsync(int categoriaId)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.CategoriaId == categoriaId && a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetByEstadoAsync(int estadoId)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.EstadoArticuloId == estadoId && a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetByEquipoAsync(string equipo)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.Equipo.Contains(equipo) && a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetAvailableAsync()
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.EstadoArticulo.Nombre == "Disponible" && a.Stock > 0 && a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetWithRelationsAsync()
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.Activo &&
                           (a.Nombre.Contains(searchTerm) ||
                            a.Codigo.Contains(searchTerm) ||
                            a.Equipo.Contains(searchTerm) ||
                            a.Descripcion!.Contains(searchTerm)))
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetFilteredAsync(int? categoriaId, int? estadoId, string? searchTerm)
        {
            var query = _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.Activo);

            if (categoriaId.HasValue)
                query = query.Where(a => a.CategoriaId == categoriaId.Value);

            if (estadoId.HasValue)
                query = query.Where(a => a.EstadoArticuloId == estadoId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Nombre.Contains(searchTerm) ||
                                        a.Codigo.Contains(searchTerm) ||
                                        a.Equipo.Contains(searchTerm));
            }

            return await query.OrderBy(a => a.Nombre).ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetPagedWithFiltersAsync(int pageNumber, int pageSize, int? categoriaId, int? estadoId, string? searchTerm, string? orderBy, bool ascending = true)
        {
            var query = _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.Activo);

            if (categoriaId.HasValue)
                query = query.Where(a => a.CategoriaId == categoriaId.Value);

            if (estadoId.HasValue)
                query = query.Where(a => a.EstadoArticuloId == estadoId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Nombre.Contains(searchTerm) ||
                                        a.Codigo.Contains(searchTerm) ||
                                        a.Equipo.Contains(searchTerm));
            }

            query = orderBy?.ToLower() switch
            {
                "codigo" => ascending ? query.OrderBy(a => a.Codigo) : query.OrderByDescending(a => a.Codigo),
                "equipo" => ascending ? query.OrderBy(a => a.Equipo) : query.OrderByDescending(a => a.Equipo),
                "precio" => ascending ? query.OrderBy(a => a.Precio) : query.OrderByDescending(a => a.Precio),
                _ => ascending ? query.OrderBy(a => a.Nombre) : query.OrderByDescending(a => a.Nombre)
            };

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo)
        {
            return await _dbSet.AnyAsync(a => a.Codigo == codigo);
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int excludeId)
        {
            return await _dbSet.AnyAsync(a => a.Codigo == codigo && a.Id != excludeId);
        }

        public async Task<bool> CanBeDeletedAsync(int id)
        {
            return !await _context.Prestamos.AnyAsync(p => p.ArticuloId == id && p.EstadoPrestamo.Nombre != "Devuelto");
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _dbSet.Where(a => a.Activo).SumAsync(a => a.Stock);
        }

        public async Task<int> GetAvailableStockAsync()
        {
            return await _dbSet
                .Where(a => a.Activo && a.EstadoArticulo.Nombre == "Disponible")
                .SumAsync(a => a.Stock);
        }

        public async Task<IEnumerable<Articulo>> GetLowStockAsync(int minStock = 1)
        {
            return await _dbSet
                .Include(a => a.Categoria)
                .Include(a => a.EstadoArticulo)
                .Where(a => a.Activo && a.Stock <= minStock)
                .OrderBy(a => a.Stock)
                .ThenBy(a => a.Nombre)
                .ToListAsync();
        }
    }
}
