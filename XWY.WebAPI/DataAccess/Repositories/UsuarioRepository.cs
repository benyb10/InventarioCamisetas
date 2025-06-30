using Microsoft.EntityFrameworkCore;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Repositories
{
    public class  UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(XWYDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Activo);
        }

        public async Task<Usuario?> GetByCedulaAsync(string cedula)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Cedula == cedula && u.Activo);
        }

        public async Task<Usuario?> GetByEmailWithRolAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo);
        }

        public async Task<Usuario?> GetByIdWithRolAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);
        }

        public async Task<IEnumerable<Usuario>> GetByRolAsync(int rolId)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .Where(u => u.RolId == rolId && u.Activo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Include(u => u.Rol)
                .Where(u => u.Activo)
                .OrderBy(u => u.Nombres)
                .ThenBy(u => u.Apellidos)
                .ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            return await _dbSet.AnyAsync(u => u.Cedula == cedula);
        }

        public async Task UpdateLastAccessAsync(int userId)
        {
            var usuario = await _dbSet.FindAsync(userId);
            if (usuario != null)
            {
                usuario.FechaUltimoAcceso = DateTime.Now;
                _dbSet.Update(usuario);
            }
        }

        public async Task<IEnumerable<Usuario>> SearchUsersAsync(string searchTerm)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .Where(u => u.Activo &&
                           (u.Nombres.Contains(searchTerm) ||
                            u.Apellidos.Contains(searchTerm) ||
                            u.Email.Contains(searchTerm) ||
                            u.Cedula.Contains(searchTerm)))
                .OrderBy(u => u.Nombres)
                .ThenBy(u => u.Apellidos)
                .ToListAsync();
        }

        public async Task<int> GetTotalActiveUsersAsync()
        {
            return await _dbSet.CountAsync(u => u.Activo);
        }
    }
}
