using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface ICatalogoService
    {
        Task<ResponseDto<List<RolDto>>> GetRolesAsync();
        Task<ResponseDto<List<CategoriaDto>>> GetCategoriasAsync();
        Task<ResponseDto<List<EstadoArticuloDto>>> GetEstadosArticuloAsync();
        Task<ResponseDto<List<EstadoPrestamoDto>>> GetEstadosPrestamoAsync();
        Task<ResponseDto<RolDto>> CreateRolAsync(RolCreateDto rolCreateDto);
        Task<ResponseDto<CategoriaDto>> CreateCategoriaAsync(CategoriaCreateDto categoriaCreateDto);
        Task<ResponseDto<EstadoArticuloDto>> CreateEstadoArticuloAsync(EstadoArticuloCreateDto estadoCreateDto);
        Task<ResponseDto<EstadoPrestamoDto>> CreateEstadoPrestamoAsync(EstadoPrestamoCreateDto estadoCreateDto);
        Task<ResponseDto<RolDto>> UpdateRolAsync(RolUpdateDto rolUpdateDto);
        Task<ResponseDto<CategoriaDto>> UpdateCategoriaAsync(CategoriaUpdateDto categoriaUpdateDto);
        Task<ResponseDto<EstadoArticuloDto>> UpdateEstadoArticuloAsync(EstadoArticuloUpdateDto estadoUpdateDto);
        Task<ResponseDto<EstadoPrestamoDto>> UpdateEstadoPrestamoAsync(EstadoPrestamoUpdateDto estadoUpdateDto);
        Task<ResponseDto<bool>> DeleteRolAsync(int id);
        Task<ResponseDto<bool>> DeleteCategoriaAsync(int id);
        Task<ResponseDto<bool>> DeleteEstadoArticuloAsync(int id);
        Task<ResponseDto<bool>> DeleteEstadoPrestamoAsync(int id);
    }
}
