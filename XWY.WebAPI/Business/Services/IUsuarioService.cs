using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface IUsuarioService
    {
        Task<ResponseDto<PagedResponseDto<UsuarioDto>>> GetAllPagedAsync(int pagina, int registrosPorPagina);
        Task<ResponseDto<UsuarioDto>> GetByIdAsync(int id);
        Task<ResponseDto<UsuarioDto>> GetByEmailAsync(string email);
        Task<ResponseDto<UsuarioDto>> CreateAsync(UsuarioCreateDto usuarioCreateDto);
        Task<ResponseDto<UsuarioDto>> UpdateAsync(UsuarioUpdateDto usuarioUpdateDto);
        Task<ResponseDto<bool>> DeleteAsync(int id);
        Task<ResponseDto<List<UsuarioDto>>> GetByRolAsync(int rolId);
        Task<ResponseDto<List<UsuarioDto>>> SearchAsync(string searchTerm);
        Task<ResponseDto<bool>> ExistsByEmailAsync(string email);
        Task<ResponseDto<bool>> ExistsByCedulaAsync(string cedula);
        Task<ResponseDto<int>> GetTotalActiveUsersAsync();
    }
}
