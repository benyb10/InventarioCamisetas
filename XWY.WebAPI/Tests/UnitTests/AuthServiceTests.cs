using AutoMapper;
using Moq;
using Xunit;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Business.Services;
using XWY.WebAPI.DataAccess.Repositories;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Tests.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();

            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x["SecretKey"]).Returns("XWY_SecretKey_2024_InventarioCamisetas_SuperSeguro_MinLength32Characters");
            configSection.Setup(x => x["Issuer"]).Returns("XWY.WebAPI");
            configSection.Setup(x => x["Audience"]).Returns("XWY.Client");
            configSection.Setup(x => x.GetValue<int>("ExpirationInMinutes")).Returns(60);

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(configSection.Object);
            _mockUnitOfWork.Setup(x => x.Usuarios).Returns(_mockUsuarioRepository.Object);

            _authService = new AuthService(_mockUnitOfWork.Object, _mockMapper.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            var loginDto = new UsuarioLoginDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Activo = true,
                Rol = new Rol { Id = 1, Nombre = "Administrador" }
            };

            var usuarioDto = new UsuarioDto
            {
                Id = 1,
                Email = "test@test.com",
                RolNombre = "Administrador"
            };

            _mockUsuarioRepository.Setup(x => x.GetByEmailWithRolAsync(loginDto.Email))
                .ReturnsAsync(usuario);
            _mockMapper.Setup(x => x.Map<UsuarioDto>(usuario)).Returns(usuarioDto);
            _mockUsuarioRepository.Setup(x => x.UpdateLastAccessAsync(usuario.Id)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _authService.LoginAsync(loginDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Token);
            Assert.Equal(usuarioDto, result.Data.Usuario);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ReturnsFailureResponse()
        {
            var loginDto = new UsuarioLoginDto
            {
                Email = "invalid@test.com",
                Password = "password123"
            };

            _mockUsuarioRepository.Setup(x => x.GetByEmailWithRolAsync(loginDto.Email))
                .ReturnsAsync((Usuario)null);

            var result = await _authService.LoginAsync(loginDto);

            Assert.False(result.Success);
            Assert.Equal("Email o contraseña incorrectos", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFailureResponse()
        {
            var loginDto = new UsuarioLoginDto
            {
                Email = "test@test.com",
                Password = "wrongpassword"
            };

            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Activo = true
            };

            _mockUsuarioRepository.Setup(x => x.GetByEmailWithRolAsync(loginDto.Email))
                .ReturnsAsync(usuario);

            var result = await _authService.LoginAsync(loginDto);

            Assert.False(result.Success);
            Assert.Equal("Email o contraseña incorrectos", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsSuccessResponse()
        {
            var createDto = new UsuarioCreateDto
            {
                Cedula = "1234567890",
                Nombres = "Test",
                Apellidos = "User",
                Email = "newuser@test.com",
                Password = "password123",
                RolId = 2
            };

            var usuario = new Usuario
            {
                Id = 1,
                Cedula = createDto.Cedula,
                Nombres = createDto.Nombres,
                Apellidos = createDto.Apellidos,
                Email = createDto.Email,
                RolId = createDto.RolId
            };

            var usuarioConRol = new Usuario
            {
                Id = 1,
                Cedula = createDto.Cedula,
                Nombres = createDto.Nombres,
                Apellidos = createDto.Apellidos,
                Email = createDto.Email,
                RolId = createDto.RolId,
                Rol = new Rol { Id = 2, Nombre = "Operador" }
            };

            var usuarioDto = new UsuarioDto
            {
                Id = 1,
                Cedula = createDto.Cedula,
                Nombres = createDto.Nombres,
                Apellidos = createDto.Apellidos,
                Email = createDto.Email,
                RolId = createDto.RolId,
                RolNombre = "Operador"
            };

            _mockUsuarioRepository.Setup(x => x.ExistsByEmailAsync(createDto.Email)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(x => x.ExistsByCedulaAsync(createDto.Cedula)).ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<Usuario>(createDto)).Returns(usuario);
            _mockUsuarioRepository.Setup(x => x.AddAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockUsuarioRepository.Setup(x => x.GetByIdWithRolAsync(It.IsAny<int>())).ReturnsAsync(usuarioConRol);
            _mockMapper.Setup(x => x.Map<UsuarioDto>(usuarioConRol)).Returns(usuarioDto);

            var result = await _authService.RegisterAsync(createDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(usuarioDto.Email, result.Data.Email);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsFailureResponse()
        {
            var createDto = new UsuarioCreateDto
            {
                Email = "existing@test.com",
                Cedula = "1234567890"
            };

            _mockUsuarioRepository.Setup(x => x.ExistsByEmailAsync(createDto.Email)).ReturnsAsync(true);

            var result = await _authService.RegisterAsync(createDto);

            Assert.False(result.Success);
            Assert.Equal("El email ya está registrado", result.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidData_ReturnsSuccessResponse()
        {
            var changePasswordDto = new UsuarioPasswordChangeDto
            {
                Id = 1,
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword123"
            };

            var usuario = new Usuario
            {
                Id = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
                Activo = true
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(changePasswordDto.Id)).ReturnsAsync(usuario);
            _mockUnitOfWork.Setup(x => x.Usuarios).Returns(_mockUsuarioRepository.Object);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _authService.ChangePasswordAsync(changePasswordDto);

            Assert.True(result.Success);
            Assert.Equal("Contraseña cambiada exitosamente", result.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ReturnsFailureResponse()
        {
            var changePasswordDto = new UsuarioPasswordChangeDto
            {
                Id = 1,
                CurrentPassword = "wrongpassword",
                NewPassword = "newpassword123"
            };

            var usuario = new Usuario
            {
                Id = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Activo = true
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(changePasswordDto.Id)).ReturnsAsync(usuario);

            var result = await _authService.ChangePasswordAsync(changePasswordDto);

            Assert.False(result.Success);
            Assert.Equal("Contraseña actual incorrecta", result.Message);
        }

        [Fact]
        public void HashPassword_ReturnsValidHash()
        {
            var password = "testpassword123";

            var hash = _authService.HashPassword(password);

            Assert.NotNull(hash);
            Assert.NotEqual(password, hash);
            Assert.True(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            var password = "testpassword123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var result = _authService.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            var password = "testpassword123";
            var wrongPassword = "wrongpassword";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var result = _authService.VerifyPassword(wrongPassword, hash);

            Assert.False(result);
        }
    }
}
