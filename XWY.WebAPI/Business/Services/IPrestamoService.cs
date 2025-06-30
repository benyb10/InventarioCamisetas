using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface IPrestamoService
    {
        Task<ResponseDto<PagedResponseDto<PrestamoDto>>> GetAllPagedAsync(PrestamoFiltroDto filtro);
        Task<ResponseDto<PrestamoDto>> GetByIdAsync(int id);
        Task<ResponseDto<PrestamoDto>> CreateAsync(PrestamoCreateDto prestamoCreateDto);
        Task<ResponseDto<PrestamoDto>> UpdateAsync(PrestamoUpdateDto prestamoUpdateDto);
        Task<ResponseDto<bool>> DeleteAsync(int id);
        Task<ResponseDto<PrestamoDto>> ApproveAsync(PrestamoAprobacionDto aprobacionDto);
        Task<ResponseDto<PrestamoDto>> RejectAsync(int id, int rechazadoPor, string observaciones);
        Task<ResponseDto<PrestamoDto>> DeliverAsync(int id, DateTime fechaEntrega);
        Task<ResponseDto<PrestamoDto>> ReturnAsync(PrestamoDevolucionDto devolucionDto);
        Task<ResponseDto<List<PrestamoDto>>> GetByUsuarioAsync(int usuarioId);
        Task<ResponseDto<List<PrestamoDto>>> GetByArticuloAsync(int articuloId);
        Task<ResponseDto<List<PrestamoDto>>> GetPendientesAsync();
        Task<ResponseDto<List<PrestamoDto>>> GetAprobadosAsync();
        Task<ResponseDto<List<PrestamoDto>>> GetEntregadosAsync();
        Task<ResponseDto<List<PrestamoDto>>> GetVencidosAsync();
        Task<ResponseDto<List<PrestamoDto>>> GetActivosAsync();
        Task<ResponseDto<List<PrestamoDto>>> GetHistorialAsync();
        Task<ResponseDto<bool>> HasActiveLoanAsync(int articuloId);
        Task<ResponseDto<bool>> UserHasActiveLoanAsync(int usuarioId, int articuloId);
        Task<ResponseDto<int>> GetTotalPrestamosAsync();
        Task<ResponseDto<int>> GetPendientesCountAsync();
        Task<ResponseDto<int>> GetVencidosCountAsync();
    }
}
