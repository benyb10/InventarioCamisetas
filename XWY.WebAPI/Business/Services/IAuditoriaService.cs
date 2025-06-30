using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface IAuditoriaService
    {
        Task<ResponseDto<PagedResponseDto<AuditoriaLogDto>>> GetAllPagedAsync(AuditoriaLogFiltroDto filtro);
        Task<ResponseDto<AuditoriaLogDto>> GetByIdAsync(int id);
        Task<ResponseDto<AuditoriaLogDto>> CreateAsync(AuditoriaLogCreateDto auditoriaCreateDto);
        Task<ResponseDto<List<AuditoriaLogDto>>> GetByUsuarioAsync(int usuarioId);
        Task<ResponseDto<List<AuditoriaLogDto>>> GetByTablaAsync(string tabla);
        Task<ResponseDto<List<AuditoriaLogDto>>> GetByAccionAsync(string accion);
        Task<ResponseDto<List<AuditoriaLogDto>>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ResponseDto<bool>> LogAccionAsync(int? usuarioId, string accion, string tabla, int? registroId, object valoresAnteriores, object valoresNuevos, string direccionIP, string userAgent);
        Task<ResponseDto<int>> GetTotalRegistrosAsync();
        Task<ResponseDto<bool>> LimpiarAuditoriaAntiguaAsync(int diasAntiguedad = 365);

    }
}
