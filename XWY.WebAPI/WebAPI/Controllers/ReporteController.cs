using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly IAuditoriaService _auditoriaService;

        public ReporteController(IReporteService reporteService, IAuditoriaService auditoriaService)
        {
            _reporteService = reporteService;
            _auditoriaService = auditoriaService;
        }

        [HttpPost("articulos/pdf")]
        public async Task<ActionResult> GenerarReporteArticulosPdf([FromBody] ReporteParametrosDto parametros)
        {
            try
            {
                parametros.TipoReporte = "articulos";
                parametros.Formato = "pdf";

                var result = await _reporteService.GenerarReporteArticulosPdfAsync(parametros);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "EXPORT",
                        "Articulos",
                        null,
                        null,
                        new { TipoReporte = "ArticulosPDF", Parametros = parametros },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    return File(result.Data, "application/pdf", $"ReporteArticulos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<byte[]>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("articulos/excel")]
        public async Task<ActionResult> GenerarReporteArticulosExcel([FromBody] ReporteParametrosDto parametros)
        {
            try
            {
                parametros.TipoReporte = "articulos";
                parametros.Formato = "excel";

                var result = await _reporteService.GenerarReporteArticulosExcelAsync(parametros);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "EXPORT",
                        "Articulos",
                        null,
                        null,
                        new { TipoReporte = "ArticulosExcel", Parametros = parametros },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReporteArticulos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<byte[]>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("prestamos/pdf")]
        public async Task<ActionResult> GenerarReportePrestamosPdf([FromBody] ReporteParametrosDto parametros)
        {
            try
            {
                parametros.TipoReporte = "prestamos";
                parametros.Formato = "pdf";

                var result = await _reporteService.GenerarReportePrestamosPdfAsync(parametros);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "EXPORT",
                        "Prestamos",
                        null,
                        null,
                        new { TipoReporte = "PrestamosPDF", Parametros = parametros },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    return File(result.Data, "application/pdf", $"ReportePrestamos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<byte[]>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("prestamos/excel")]
        public async Task<ActionResult> GenerarReportePrestamosExcel([FromBody] ReporteParametrosDto parametros)
        {
            try
            {
                parametros.TipoReporte = "prestamos";
                parametros.Formato = "excel";

                var result = await _reporteService.GenerarReportePrestamosExcelAsync(parametros);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "EXPORT",
                        "Prestamos",
                        null,
                        null,
                        new { TipoReporte = "PrestamosExcel", Parametros = parametros },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReportePrestamos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<byte[]>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("personalizado")]
        public async Task<ActionResult> GenerarReportePersonalizado([FromBody] ReporteParametrosDto parametros)
        {
            try
            {
                var result = await _reporteService.GenerarReportePersonalizadoAsync(parametros);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "EXPORT",
                        parametros.TipoReporte == "articulos" ? "Articulos" : "Prestamos",
                        null,
                        null,
                        new { TipoReporte = "Personalizado", Parametros = parametros },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    var contentType = parametros.Formato?.ToLower() == "excel"
                        ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        : "application/pdf";

                    var extension = parametros.Formato?.ToLower() == "excel" ? "xlsx" : "pdf";
                    var fileName = $"Reporte_{parametros.TipoReporte}_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";

                    return File(result.Data, contentType, fileName);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<byte[]>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("articulos/datos")]
        public async Task<ActionResult<ResponseDto<List<ReporteArticulosDto>>>> ObtenerDatosReporteArticulos([FromQuery] ReporteParametrosDto parametros)
        {
            try
            {
                var result = await _reporteService.ObtenerDatosReporteArticulosAsync(parametros);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ReporteArticulosDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("prestamos/datos")]
        public async Task<ActionResult<ResponseDto<List<ReportePrestamosDto>>>> ObtenerDatosReportePrestamos([FromQuery] ReporteParametrosDto parametros)
        {
            try
            {
                var result = await _reporteService.ObtenerDatosReportePrestamosAsync(parametros);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ReportePrestamosDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}
