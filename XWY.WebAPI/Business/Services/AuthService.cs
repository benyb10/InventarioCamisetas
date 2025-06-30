using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(UsuarioLoginDto loginDto)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByEmailWithRolAsync(loginDto.Email);

                if (usuario == null || !usuario.Activo)
                {
                    return new ResponseDto<LoginResponseDto>("Email o contraseña incorrectos");
                }

                if (!VerifyPassword(loginDto.Password, usuario.PasswordHash))
                {
                    return new ResponseDto<LoginResponseDto>("Email o contraseña incorrectos");
                }

                await _unitOfWork.Usuarios.UpdateLastAccessAsync(usuario.Id);
                await _unitOfWork.SaveChangesAsync();

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
                var tokenResult = await GenerateJwtTokenAsync(usuarioDto);

                if (!tokenResult.Success)
                {
                    return new ResponseDto<LoginResponseDto>("Error al generar token");
                }

                var loginResponse = new LoginResponseDto
                {
                    Token = tokenResult.Data,
                    Expiration = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationInMinutes")),
                    Usuario = usuarioDto
                };

                return new ResponseDto<LoginResponseDto>(loginResponse, "Login exitoso");
            }
            catch (Exception ex)
            {
                return new ResponseDto<LoginResponseDto>($"Error en el login: {ex.Message}");
            }
        }

        public async Task<ResponseDto<string>> GenerateJwtTokenAsync(UsuarioDto usuario)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];
                var expirationInMinutes = jwtSettings.GetValue<int>("ExpirationInMinutes");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombres+" "+ usuario.Apellidos),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.RolNombre),
                    new Claim("RolId", usuario.RolId.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return new ResponseDto<string>(tokenString, "Token generado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>($"Error al generar token: {ex.Message}");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> RegisterAsync(UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                if (await _unitOfWork.Usuarios.ExistsByEmailAsync(usuarioCreateDto.Email))
                {
                    return new ResponseDto<UsuarioDto>("El email ya está registrado");
                }

                if (await _unitOfWork.Usuarios.ExistsByCedulaAsync(usuarioCreateDto.Cedula))
                {
                    return new ResponseDto<UsuarioDto>("La cédula ya está registrada");
                }

                var usuario = _mapper.Map<Usuario>(usuarioCreateDto);
                usuario.PasswordHash = HashPassword(usuarioCreateDto.Password);
                usuario.FechaCreacion = DateTime.Now;

                await _unitOfWork.Usuarios.AddAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                var usuarioConRol = await _unitOfWork.Usuarios.GetByIdWithRolAsync(usuario.Id);
                var usuarioDto = _mapper.Map<UsuarioDto>(usuarioConRol);

                return new ResponseDto<UsuarioDto>(usuarioDto, "Usuario registrado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<UsuarioDto>($"Error al registrar usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ChangePasswordAsync(UsuarioPasswordChangeDto changePasswordDto)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(changePasswordDto.Id);

                if (usuario == null || !usuario.Activo)
                {
                    return new ResponseDto<bool>("Usuario no encontrado");
                }

                if (!VerifyPassword(changePasswordDto.CurrentPassword, usuario.PasswordHash))
                {
                    return new ResponseDto<bool>("Contraseña actual incorrecta");
                }

                usuario.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                _unitOfWork.Usuarios.Update(usuario);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al cambiar contraseña: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return new ResponseDto<bool>(true, "Token válido");
            }
            catch (Exception)
            {
                return new ResponseDto<bool>(false, "Token inválido");
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
