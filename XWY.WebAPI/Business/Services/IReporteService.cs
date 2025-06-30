using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface IReporteService
    {
        Task<ResponseDto<byte[]>> GenerarReporteArticulosPdfAsync(ReporteParametrosDto parametros);
        Task<ResponseDto<byte[]>> GenerarReporteArticulosExcelAsync(ReporteParametrosDto parametros);
        Task<ResponseDto<byte[]>> GenerarReportePrestamosPdfAsync(ReporteParametrosDto parametros);
        Task<ResponseDto<byte[]>> GenerarReportePrestamosExcelAsync(ReporteParametrosDto parametros);
        Task<ResponseDto<List<ReporteArticulosDto>>> ObtenerDatosReporteArticulosAsync(ReporteParametrosDto parametros);
        Task<ResponseDto<List<ReportePrestamosDto>>> ObtenerDatosReportePrestamosAsync(ReporteParametrosDto parametros);
        Task<ResponseDto<byte[]>> GenerarReportePersonalizadoAsync(ReporteParametrosDto parametros);

    }
}
