using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Repositories
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByCedulaAsync(string cedula);
        Task<Usuario?> GetByEmailWithRolAsync(string email);
        Task<Usuario?> GetByIdWithRolAsync(int id);
        Task<IEnumerable<Usuario>> GetByRolAsync(int rolId);
        Task<IEnumerable<Usuario>> GetActiveUsersAsync();
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByCedulaAsync(string cedula);
        Task UpdateLastAccessAsync(int userId);
        Task<IEnumerable<Usuario>> SearchUsersAsync(string searchTerm);
        Task<int> GetTotalActiveUsersAsync();
    }
}
