using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticuloController : ControllerBase
    {
        private readonly IArticuloService _articuloService;
        private readonly IAuditoriaService _auditoriaService;

        public ArticuloController(IArticuloService articuloService, IAuditoriaService auditoriaService)
        {
            _articuloService = articuloService;
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PagedResponseDto<ArticuloDto>>>> GetAll([FromQuery] ArticuloFiltroDto filtro)
        {
            try
            {
                var result = await _articuloService.GetAllPagedAsync(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PagedResponseDto<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<ArticuloDto>>> GetById(int id)
        {
            try
            {
                var result = await _articuloService.GetByIdAsync(id);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<ArticuloDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<ActionResult<ResponseDto<ArticuloDto>>> GetByCodigo(string codigo)
        {
            try
            {
                var result = await _articuloService.GetByCodigoAsync(codigo);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<ArticuloDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<ArticuloDto>>> Create([FromBody] ArticuloCreateDto articuloCreateDto)
        {
            try
            {
                var result = await _articuloService.CreateAsync(articuloCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "CREATE",
                        "Articulos",
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
                return StatusCode(500, new ResponseDto<ArticuloDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<ArticuloDto>>> Update(int id, [FromBody] ArticuloUpdateDto articuloUpdateDto)
        {
            try
            {
                if (id != articuloUpdateDto.Id)
                {
                    return BadRequest(new ResponseDto<ArticuloDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var articuloAnterior = await _articuloService.GetByIdAsync(id);
                var result = await _articuloService.UpdateAsync(articuloUpdateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "UPDATE",
                        "Articulos",
                        id,
                        articuloAnterior.Data,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<ArticuloDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> Delete(int id)
        {
            try
            {
                var articuloAnterior = await _articuloService.GetByIdAsync(id);
                var result = await _articuloService.DeleteAsync(id);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "DELETE",
                        "Articulos",
                        id,
                        articuloAnterior.Data,
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

        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloDto>>>> GetByCategoria(int categoriaId)
        {
            try
            {
                var result = await _articuloService.GetByCategoriaAsync(categoriaId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("estado/{estadoId}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloDto>>>> GetByEstado(int estadoId)
        {
            try
            {
                var result = await _articuloService.GetByEstadoAsync(estadoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("equipo/{equipo}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloDto>>>> GetByEquipo(string equipo)
        {
            try
            {
                var result = await _articuloService.GetByEquipoAsync(equipo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<ResponseDto<List<ArticuloDto>>>> GetAvailable()
        {
            try
            {
                var result = await _articuloService.GetAvailableAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<ResponseDto<List<ArticuloDto>>>> Search(string searchTerm)
        {
            try
            {
                var result = await _articuloService.SearchAsync(searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<ResponseDto<List<ArticuloDto>>>> GetLowStock([FromQuery] int minStock = 1)
        {
            try
            {
                var result = await _articuloService.GetLowStockAsync(minStock);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<ArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("total-stock")]
        public async Task<ActionResult<ResponseDto<int>>> GetTotalStock()
        {
            try
            {
                var result = await _articuloService.GetTotalStockAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("available-stock")]
        public async Task<ActionResult<ResponseDto<int>>> GetAvailableStock()
        {
            try
            {
                var result = await _articuloService.GetAvailableStockAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}
