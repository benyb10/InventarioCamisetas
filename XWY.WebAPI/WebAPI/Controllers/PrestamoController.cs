using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamoController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;
        private readonly IAuditoriaService _auditoriaService;

        public PrestamoController(IPrestamoService prestamoService, IAuditoriaService auditoriaService)
        {
            _prestamoService = prestamoService;
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PagedResponseDto<PrestamoDto>>>> GetAll([FromQuery] PrestamoFiltroDto filtro)
        {
            try
            {
                var result = await _prestamoService.GetAllPagedAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PagedResponseDto<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> GetById(int id)
        {
            try
            {
                var result = await _prestamoService.GetByIdAsync(id);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> Create([FromBody] PrestamoCreateDto prestamoCreateDto)
        {
            try
            {
                var result = await _prestamoService.CreateAsync(prestamoCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        prestamoCreateDto.UsuarioId,
                        "CREATE",
                        "Prestamos",
                        result.Data.Id,
                        null,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> Update(int id, [FromBody] PrestamoUpdateDto prestamoUpdateDto)
        {
            try
            {
                if (id != prestamoUpdateDto.Id)
                {
                    return BadRequest(new ResponseDto<PrestamoDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var prestamoAnterior = await _prestamoService.GetByIdAsync(id);
                var result = await _prestamoService.UpdateAsync(prestamoUpdateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        result.Data.UsuarioId,
                        "UPDATE",
                        "Prestamos",
                        id,
                        prestamoAnterior.Data,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> Delete(int id)
        {
            try
            {
                var prestamoAnterior = await _prestamoService.GetByIdAsync(id);
                var result = await _prestamoService.DeleteAsync(id);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        prestamoAnterior.Data?.UsuarioId,
                        "DELETE",
                        "Prestamos",
                        id,
                        prestamoAnterior.Data,
                        null,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<bool>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> Approve(int id, [FromBody] PrestamoAprobacionDto aprobacionDto)
        {
            try
            {
                if (id != aprobacionDto.Id)
                {
                    return BadRequest(new ResponseDto<PrestamoDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var result = await _prestamoService.ApproveAsync(aprobacionDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        aprobacionDto.AprobadoPor,
                        "APPROVE",
                        "Prestamos",
                        id,
                        null,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> Reject(int id, [FromBody] PrestamoAprobacionDto rechazoDto)
        {
            try
            {
                var result = await _prestamoService.RejectAsync(id, rechazoDto.AprobadoPor, rechazoDto.ObservacionesAprobacion);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        rechazoDto.AprobadoPor,
                        "REJECT",
                        "Prestamos",
                        id,
                        null,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("{id}/deliver")]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> Deliver(int id, [FromBody] DateTime fechaEntrega)
        {
            try
            {
                var result = await _prestamoService.DeliverAsync(id, fechaEntrega);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        result.Data.UsuarioId,
                        "DELIVER",
                        "Prestamos",
                        id,
                        null,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("{id}/return")]
        public async Task<ActionResult<ResponseDto<PrestamoDto>>> Return(int id, [FromBody] PrestamoDevolucionDto devolucionDto)
        {
            try
            {
                if (id != devolucionDto.Id)
                {
                    return BadRequest(new ResponseDto<PrestamoDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var result = await _prestamoService.ReturnAsync(devolucionDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        result.Data.UsuarioId,
                        "RETURN",
                        "Prestamos",
                        id,
                        null,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<ResponseDto<List<PrestamoDto>>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var result = await _prestamoService.GetByUsuarioAsync(usuarioId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("articulo/{articuloId}")]
        public async Task<ActionResult<ResponseDto<List<PrestamoDto>>>> GetByArticulo(int articuloId)
        {
            try
            {
                var result = await _prestamoService.GetByArticuloAsync(articuloId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult<ResponseDto<List<PrestamoDto>>>> GetPendientes()
        {
            try
            {
                var result = await _prestamoService.GetPendientesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("vencidos")]
        public async Task<ActionResult<ResponseDto<List<PrestamoDto>>>> GetVencidos()
        {
            try
            {
                var result = await _prestamoService.GetVencidosAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("activos")]
        public async Task<ActionResult<ResponseDto<List<PrestamoDto>>>> GetActivos()
        {
            try
            {
                var result = await _prestamoService.GetActivosAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("historial")]
        public async Task<ActionResult<ResponseDto<List<PrestamoDto>>>> GetHistorial()
        {
            try
            {
                var result = await _prestamoService.GetHistorialAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<PrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("total")]
        public async Task<ActionResult<ResponseDto<int>>> GetTotal()
        {
            try
            {
                var result = await _prestamoService.GetTotalPrestamosAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("pendientes-count")]
        public async Task<ActionResult<ResponseDto<int>>> GetPendientesCount()
        {
            try
            {
                var result = await _prestamoService.GetPendientesCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("vencidos-count")]
        public async Task<ActionResult<ResponseDto<int>>> GetVencidosCount()
        {
            try
            {
                var result = await _prestamoService.GetVencidosCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}
