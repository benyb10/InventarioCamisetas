using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;
        private readonly IAuditoriaService _auditoriaService;

        public CatalogoController(ICatalogoService catalogoService, IAuditoriaService auditoriaService)
        {
            _catalogoService = catalogoService;
            _auditoriaService = auditoriaService;
        }

        [HttpGet("roles")]
        public async Task<ActionResult<ResponseDto<List<RolDto>>>> GetRoles()
        {
            try
            {
                var result = await _catalogoService.GetRolesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<RolDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<ResponseDto<List<CategoriaDto>>>> GetCategorias()
        {
            try
            {
                var result = await _catalogoService.GetCategoriasAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<CategoriaDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("estados-articulo")]
        public async Task<ActionResult<ResponseDto<List<EstadoArticuloDto>>>> GetEstadosArticulo()
        {
            try
            {
                var result = await _catalogoService.GetEstadosArticuloAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<EstadoArticuloDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("estados-prestamo")]
        public async Task<ActionResult<ResponseDto<List<EstadoPrestamoDto>>>> GetEstadosPrestamo()
        {
            try
            {
                var result = await _catalogoService.GetEstadosPrestamoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<EstadoPrestamoDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("roles")]
        public async Task<ActionResult<ResponseDto<RolDto>>> CreateRol([FromBody] RolCreateDto rolCreateDto)
        {
            try
            {
                var result = await _catalogoService.CreateRolAsync(rolCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "CREATE",
                        "Roles",
                        result.Data.Id,
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
                return StatusCode(500, new ResponseDto<RolDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("categorias")]
        public async Task<ActionResult<ResponseDto<CategoriaDto>>> CreateCategoria([FromBody] CategoriaCreateDto categoriaCreateDto)
        {
            try
            {
                var result = await _catalogoService.CreateCategoriaAsync(categoriaCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "CREATE",
                        "Categorias",
                        result.Data.Id,
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
                return StatusCode(500, new ResponseDto<CategoriaDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("estados-articulo")]
        public async Task<ActionResult<ResponseDto<EstadoArticuloDto>>> CreateEstadoArticulo([FromBody] EstadoArticuloCreateDto estadoCreateDto)
        {
            try
            {
                var result = await _catalogoService.CreateEstadoArticuloAsync(estadoCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "CREATE",
                        "EstadosArticulo",
                        result.Data.Id,
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
                return StatusCode(500, new ResponseDto<EstadoArticuloDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("estados-prestamo")]
        public async Task<ActionResult<ResponseDto<EstadoPrestamoDto>>> CreateEstadoPrestamo([FromBody] EstadoPrestamoCreateDto estadoCreateDto)
        {
            try
            {
                var result = await _catalogoService.CreateEstadoPrestamoAsync(estadoCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "CREATE",
                        "EstadosPrestamo",
                        result.Data.Id,
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
                return StatusCode(500, new ResponseDto<EstadoPrestamoDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPut("roles/{id}")]
        public async Task<ActionResult<ResponseDto<RolDto>>> UpdateRol(int id, [FromBody] RolUpdateDto rolUpdateDto)
        {
            try
            {
                if (id != rolUpdateDto.Id)
                {
                    return BadRequest(new ResponseDto<RolDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var result = await _catalogoService.UpdateRolAsync(rolUpdateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "UPDATE",
                        "Roles",
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
                return StatusCode(500, new ResponseDto<RolDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPut("categorias/{id}")]
        public async Task<ActionResult<ResponseDto<CategoriaDto>>> UpdateCategoria(int id, [FromBody] CategoriaUpdateDto categoriaUpdateDto)
        {
            try
            {
                if (id != categoriaUpdateDto.Id)
                {
                    return BadRequest(new ResponseDto<CategoriaDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var result = await _catalogoService.UpdateCategoriaAsync(categoriaUpdateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "UPDATE",
                        "Categorias",
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
                return StatusCode(500, new ResponseDto<CategoriaDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpDelete("roles/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteRol(int id)
        {
            try
            {
                var result = await _catalogoService.DeleteRolAsync(id);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "DELETE",
                        "Roles",
                        id,
                        null,
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

        [HttpDelete("categorias/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteCategoria(int id)
        {
            try
            {
                var result = await _catalogoService.DeleteCategoriaAsync(id);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "DELETE",
                        "Categorias",
                        id,
                        null,
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
    }
}
