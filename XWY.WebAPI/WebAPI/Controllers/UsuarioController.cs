using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAuditoriaService _auditoriaService;

        public UsuarioController(IUsuarioService usuarioService, IAuditoriaService auditoriaService)
        {
            _usuarioService = usuarioService;
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PagedResponseDto<UsuarioDto>>>> GetAll([FromQuery] int pagina = 1, [FromQuery] int registrosPorPagina = 10)
        {
            try
            {
                var result = await _usuarioService.GetAllPagedAsync(pagina, registrosPorPagina);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<PagedResponseDto<UsuarioDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<UsuarioDto>>> GetById(int id)
        {
            try
            {
                var result = await _usuarioService.GetByIdAsync(id);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<UsuarioDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<ResponseDto<UsuarioDto>>> GetByEmail(string email)
        {
            try
            {
                var result = await _usuarioService.GetByEmailAsync(email);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<UsuarioDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<UsuarioDto>>> Create([FromBody] UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                var result = await _usuarioService.CreateAsync(usuarioCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        result.Data.Id,
                        "CREATE",
                        "Usuarios",
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
                return StatusCode(500, new ResponseDto<UsuarioDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<UsuarioDto>>> Update(int id, [FromBody] UsuarioUpdateDto usuarioUpdateDto)
        {
            try
            {
                if (id != usuarioUpdateDto.Id)
                {
                    return BadRequest(new ResponseDto<UsuarioDto>("El ID de la URL no coincide con el ID del objeto"));
                }

                var usuarioAnterior = await _usuarioService.GetByIdAsync(id);
                var result = await _usuarioService.UpdateAsync(usuarioUpdateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        id,
                        "UPDATE",
                        "Usuarios",
                        id,
                        usuarioAnterior.Data,
                        result.Data,
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<UsuarioDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> Delete(int id)
        {
            try
            {
                var usuarioAnterior = await _usuarioService.GetByIdAsync(id);
                var result = await _usuarioService.DeleteAsync(id);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        id,
                        "DELETE",
                        "Usuarios",
                        id,
                        usuarioAnterior.Data,
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

        [HttpGet("rol/{rolId}")]
        public async Task<ActionResult<ResponseDto<List<UsuarioDto>>>> GetByRol(int rolId)
        {
            try
            {
                var result = await _usuarioService.GetByRolAsync(rolId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<UsuarioDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<ResponseDto<List<UsuarioDto>>>> Search(string searchTerm)
        {
            try
            {
                var result = await _usuarioService.SearchAsync(searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<List<UsuarioDto>>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("total-active")]
        public async Task<ActionResult<ResponseDto<int>>> GetTotalActiveUsers()
        {
            try
            {
                var result = await _usuarioService.GetTotalActiveUsersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<int>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}
