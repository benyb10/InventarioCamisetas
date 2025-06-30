using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PagedResponseDto<AuditoriaLogDto>>>> GetAll([FromQuery] AuditoriaLogFiltroDto filtro)
        {
            try
            {
                var result = await _auditoriaService.GetAllPagedAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PagedResponseDto<AuditoriaLogDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<AuditoriaLogDto>>> GetById(int id)
        {
            try
            {
                var result = await _auditoriaService.GetByIdAsync(id);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<AuditoriaLogDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<ResponseDto<List<AuditoriaLogDto>>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var result = await _auditoriaService.GetByUsuarioAsync(usuarioId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<AuditoriaLogDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("tabla/{tabla}")]
        public async Task<ActionResult<ResponseDto<List<AuditoriaLogDto>>>> GetByTabla(string tabla)
        {
            try
            {
                var result = await _auditoriaService.GetByTablaAsync(tabla);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<AuditoriaLogDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("accion/{accion}")]
        public async Task<ActionResult<ResponseDto<List<AuditoriaLogDto>>>> GetByAccion(string accion)
        {
            try
            {
                var result = await _auditoriaService.GetByAccionAsync(accion);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<AuditoriaLogDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("fecha-rango")]
        public async Task<ActionResult<ResponseDto<List<AuditoriaLogDto>>>> GetByFechaRango([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var result = await _auditoriaService.GetByFechaRangoAsync(fechaInicio, fechaFin);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<AuditoriaLogDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("total-registros")]
        public async Task<ActionResult<ResponseDto<int>>> GetTotalRegistros()
        {
            try
            {
                var result = await _auditoriaService.GetTotalRegistrosAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpDelete("limpiar-antigua")]
        public async Task<ActionResult<ResponseDto<bool>>> LimpiarAuditoriaAntigua([FromQuery] int diasAntiguedad = 365)
        {
            try
            {
                var result = await _auditoriaService.LimpiarAuditoriaAntiguaAsync(diasAntiguedad);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<bool>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}
