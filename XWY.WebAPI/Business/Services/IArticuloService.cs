using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface IArticuloService
    {
        Task<ResponseDto<PagedResponseDto<ArticuloDto>>> GetAllPagedAsync(ArticuloFiltroDto filtro);
        Task<ResponseDto<ArticuloDto>> GetByIdAsync(int id);
        Task<ResponseDto<ArticuloDto>> GetByCodigoAsync(string codigo);
        Task<ResponseDto<ArticuloDto>> CreateAsync(ArticuloCreateDto articuloCreateDto);
        Task<ResponseDto<ArticuloDto>> UpdateAsync(ArticuloUpdateDto articuloUpdateDto);
        Task<ResponseDto<bool>> DeleteAsync(int id);
        Task<ResponseDto<List<ArticuloDto>>> GetByCategoriaAsync(int categoriaId);
        Task<ResponseDto<List<ArticuloDto>>> GetByEstadoAsync(int estadoId);
        Task<ResponseDto<List<ArticuloDto>>> GetByEquipoAsync(string equipo);
        Task<ResponseDto<List<ArticuloDto>>> GetAvailableAsync();
        Task<ResponseDto<List<ArticuloDto>>> SearchAsync(string searchTerm);
        Task<ResponseDto<bool>> ExistsByCodigoAsync(string codigo);
        Task<ResponseDto<bool>> CanBeDeletedAsync(int id);
        Task<ResponseDto<int>> GetTotalStockAsync();
        Task<ResponseDto<int>> GetAvailableStockAsync();
        Task<ResponseDto<List<ArticuloDto>>> GetLowStockAsync(int minStock = 1);

    }
}
