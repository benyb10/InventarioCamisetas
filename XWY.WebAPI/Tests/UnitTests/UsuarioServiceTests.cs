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
    public class UsuarioServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthService = new Mock<IAuthService>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();

            _mockUnitOfWork.Setup(x => x.Usuarios).Returns(_mockUsuarioRepository.Object);

            _usuarioService = new UsuarioService(_mockUnitOfWork.Object, _mockMapper.Object, _mockAuthService.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingUser_ReturnsSuccessResponse()
        {
            var userId = 1;
            var usuario = new Usuario
            {
                Id = userId,
                Nombres = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                Activo = true,
                Rol = new Rol { Id = 1, Nombre = "Administrador" }
            };

            var usuarioDto = new UsuarioDto
            {
                Id = userId,
                Nombres = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                RolNombre = "Administrador"
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdWithRolAsync(userId)).ReturnsAsync(usuario);
            _mockMapper.Setup(x => x.Map<UsuarioDto>(usuario)).Returns(usuarioDto);

            var result = await _usuarioService.GetByIdAsync(userId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.Id);
            Assert.Equal("test@test.com", result.Data.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentUser_ReturnsFailureResponse()
        {
            var userId = 999;

            _mockUsuarioRepository.Setup(x => x.GetByIdWithRolAsync(userId)).ReturnsAsync((Usuario)null);

            var result = await _usuarioService.GetByIdAsync(userId);

            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsSuccessResponse()
        {
            var createDto = new UsuarioCreateDto
            {
                Cedula = "1234567890",
                Nombres = "Nuevo",
                Apellidos = "Usuario",
                Email = "nuevo@test.com",
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
            _mockAuthService.Setup(x => x.HashPassword(createDto.Password)).Returns("hashedpassword");
            _mockUsuarioRepository.Setup(x => x.AddAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockUsuarioRepository.Setup(x => x.GetByIdWithRolAsync(It.IsAny<int>())).ReturnsAsync(usuarioConRol);
            _mockMapper.Setup(x => x.Map<UsuarioDto>(usuarioConRol)).Returns(usuarioDto);

            var result = await _usuarioService.CreateAsync(createDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(createDto.Email, result.Data.Email);
            Assert.Equal("Usuario creado exitosamente", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithExistingEmail_ReturnsFailureResponse()
        {
            var createDto = new UsuarioCreateDto
            {
                Email = "existing@test.com",
                Cedula = "1234567890"
            };

            _mockUsuarioRepository.Setup(x => x.ExistsByEmailAsync(createDto.Email)).ReturnsAsync(true);

            var result = await _usuarioService.CreateAsync(createDto);

            Assert.False(result.Success);
            Assert.Equal("El email ya está registrado", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithExistingCedula_ReturnsFailureResponse()
        {
            var createDto = new UsuarioCreateDto
            {
                Email = "new@test.com",
                Cedula = "1234567890"
            };

            _mockUsuarioRepository.Setup(x => x.ExistsByEmailAsync(createDto.Email)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(x => x.ExistsByCedulaAsync(createDto.Cedula)).ReturnsAsync(true);

            var result = await _usuarioService.CreateAsync(createDto);

            Assert.False(result.Success);
            Assert.Equal("La cédula ya está registrada", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ReturnsSuccessResponse()
        {
            var updateDto = new UsuarioUpdateDto
            {
                Id = 1,
                Cedula = "1234567890",
                Nombres = "Updated",
                Apellidos = "User",
                Email = "updated@test.com",
                RolId = 2,
                Activo = true
            };

            var existingUsuario = new Usuario
            {
                Id = 1,
                Cedula = "1234567890",
                Nombres = "Original",
                Apellidos = "User",
                Email = "original@test.com",
                RolId = 1
            };

            var updatedUsuario = new Usuario
            {
                Id = 1,
                Cedula = updateDto.Cedula,
                Nombres = updateDto.Nombres,
                Apellidos = updateDto.Apellidos,
                Email = updateDto.Email,
                RolId = updateDto.RolId,
                Rol = new Rol { Id = 2, Nombre = "Operador" }
            };

            var usuarioDto = new UsuarioDto
            {
                Id = 1,
                Cedula = updateDto.Cedula,
                Nombres = updateDto.Nombres,
                Apellidos = updateDto.Apellidos,
                Email = updateDto.Email,
                RolId = updateDto.RolId,
                RolNombre = "Operador"
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(updateDto.Id)).ReturnsAsync(existingUsuario);
            _mockUsuarioRepository.Setup(x => x.ExistsByEmailAsync(updateDto.Email)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(x => x.ExistsByCedulaAsync(updateDto.Cedula)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockUsuarioRepository.Setup(x => x.GetByIdWithRolAsync(updateDto.Id)).ReturnsAsync(updatedUsuario);
            _mockMapper.Setup(x => x.Map<UsuarioDto>(updatedUsuario)).Returns(usuarioDto);

            var result = await _usuarioService.UpdateAsync(updateDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(updateDto.Email, result.Data.Email);
            Assert.Equal("Usuario actualizado exitosamente", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingUser_ReturnsSuccessResponse()
        {
            var userId = 1;
            var usuario = new Usuario
            {
                Id = userId,
                Nombres = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                Activo = true
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(usuario);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _usuarioService.DeleteAsync(userId);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Usuario eliminado exitosamente", result.Message);
            _mockUsuarioRepository.Verify(x => x.Update(It.Is<Usuario>(u => u.Activo == false)), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_WithValidTerm_ReturnsUsers()
        {
            var searchTerm = "test";
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Nombres = "Test",
                    Apellidos = "User",
                    Email = "test@test.com",
                    Rol = new Rol { Id = 1, Nombre = "Administrador" }
                }
            };

            var usuariosDto = new List<UsuarioDto>
            {
                new UsuarioDto
                {
                    Id = 1,
                    Nombres = "Test",
                    Apellidos = "User",
                    Email = "test@test.com",
                    RolNombre = "Administrador"
                }
            };

            _mockUsuarioRepository.Setup(x => x.SearchUsersAsync(searchTerm)).ReturnsAsync(usuarios);
            _mockMapper.Setup(x => x.Map<List<UsuarioDto>>(usuarios)).Returns(usuariosDto);

            var result = await _usuarioService.SearchAsync(searchTerm);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("test@test.com", result.Data.First().Email);
        }

        [Fact]
        public async Task GetByRolAsync_WithValidRoleId_ReturnsUsersInRole()
        {
            var rolId = 1;
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Nombres = "Admin",
                    Apellidos = "User",
                    Email = "admin@test.com",
                    RolId = rolId,
                    Rol = new Rol { Id = rolId, Nombre = "Administrador" }
                }
            };

            var usuariosDto = new List<UsuarioDto>
            {
                new UsuarioDto
                {
                    Id = 1,
                    Nombres = "Admin",
                    Apellidos = "User",
                    Email = "admin@test.com",
                    RolId = rolId,
                    RolNombre = "Administrador"
                }
            };

            _mockUsuarioRepository.Setup(x => x.GetByRolAsync(rolId)).ReturnsAsync(usuarios);
            _mockMapper.Setup(x => x.Map<List<UsuarioDto>>(usuarios)).Returns(usuariosDto);

            var result = await _usuarioService.GetByRolAsync(rolId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal(rolId, result.Data.First().RolId);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WithExistingEmail_ReturnsTrue()
        {
            var email = "existing@test.com";

            _mockUsuarioRepository.Setup(x => x.ExistsByEmailAsync(email)).ReturnsAsync(true);

            var result = await _usuarioService.ExistsByEmailAsync(email);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task ExistsByCedulaAsync_WithExistingCedula_ReturnsTrue()
        {
            var cedula = "1234567890";

            _mockUsuarioRepository.Setup(x => x.ExistsByCedulaAsync(cedula)).ReturnsAsync(true);

            var result = await _usuarioService.ExistsByCedulaAsync(cedula);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task GetTotalActiveUsersAsync_ReturnsCorrectCount()
        {
            var expectedCount = 25;

            _mockUsuarioRepository.Setup(x => x.GetTotalActiveUsersAsync()).ReturnsAsync(expectedCount);

            var result = await _usuarioService.GetTotalActiveUsersAsync();

            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data);
        }
    }
}
