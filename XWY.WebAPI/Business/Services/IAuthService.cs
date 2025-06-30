using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Services
{
    public interface IAuthService
    {
        Task<ResponseDto<LoginResponseDto>> LoginAsync(UsuarioLoginDto loginDto);
        Task<ResponseDto<string>> GenerateJwtTokenAsync(UsuarioDto usuario);
        Task<ResponseDto<UsuarioDto>> RegisterAsync(UsuarioCreateDto usuarioCreateDto);
        Task<ResponseDto<bool>> ChangePasswordAsync(UsuarioPasswordChangeDto changePasswordDto);
        Task<ResponseDto<bool>> ValidateTokenAsync(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
