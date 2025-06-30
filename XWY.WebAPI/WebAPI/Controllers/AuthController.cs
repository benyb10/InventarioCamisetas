using Microsoft.AspNetCore.Mvc;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;

namespace XWY.WebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuditoriaService _auditoriaService;

        public AuthController(IAuthService authService, IAuditoriaService auditoriaService)
        {
            _authService = authService;
            _auditoriaService = auditoriaService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login([FromBody] UsuarioLoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (result.Success)
                {
                    // Log exitoso DESPUÉS de obtener el resultado
                    await _auditoriaService.LogAccionAsync(
                        result.Data.Usuario.Id,
                        "LOGIN",
                        "Usuarios",
                        result.Data.Usuario.Id,
                        null,
                        new { Email = loginDto.Email },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    // IMPORTANTE: Asegurar que se devuelve el resultado completo
                    return Ok(result);
                }
                else
                {
                    // Log de intento fallido
                    await _auditoriaService.LogAccionAsync(
                        null,
                        "LOGIN_FAILED",
                        "Usuarios",
                        null,
                        null,
                        new { Email = loginDto.Email, Error = result.Message },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                // Log de error
                await _auditoriaService.LogAccionAsync(
                    null,
                    "LOGIN_ERROR",
                    "Usuarios",
                    null,
                    null,
                    new { Email = loginDto.Email, Error = ex.Message },
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                return StatusCode(500, new ResponseDto<LoginResponseDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto<UsuarioDto>>> Register([FromBody] UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(usuarioCreateDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        result.Data.Id,
                        "REGISTER",
                        "Usuarios",
                        result.Data.Id,
                        null,
                        new { Email = usuarioCreateDto.Email, Nombres = usuarioCreateDto.Nombres },
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );

                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<UsuarioDto>($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ResponseDto<bool>>> ChangePassword([FromBody] UsuarioPasswordChangeDto changePasswordDto)
        {
            try
            {
                var result = await _authService.ChangePasswordAsync(changePasswordDto);

                if (result.Success)
                {
                    await _auditoriaService.LogAccionAsync(
                        changePasswordDto.Id,
                        "CHANGE_PASSWORD",
                        "Usuarios",
                        changePasswordDto.Id,
                        null,
                        new { UsuarioId = changePasswordDto.Id },
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

        [HttpPost("validate-token")]
        public async Task<ActionResult<ResponseDto<bool>>> ValidateToken([FromBody] string token)
        {
            try
            {
                var result = await _authService.ValidateTokenAsync(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<bool>($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}