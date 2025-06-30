using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Tests.IntegrationTests
{
    public class UsuarioIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UsuarioIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<XWYDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<XWYDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_Usuario");
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<XWYDbContext>();

                    db.Database.EnsureCreated();
                    SeedTestData(db);
                });
            });

            _client = _factory.CreateClient();
        }

        private static void SeedTestData(XWYDbContext context)
        {
            if (context.Roles.Any()) return;

            var roles = new List<Rol>
            {
                new Rol { Id = 1, Nombre = "Administrador", Descripcion = "Admin", Activo = true },
                new Rol { Id = 2, Nombre = "Operador", Descripcion = "Operator", Activo = true }
            };

            context.Roles.AddRange(roles);

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Cedula = "1234567890",
                    Nombres = "Test",
                    Apellidos = "Admin",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RolId = 1,
                    Activo = true
                },
                new Usuario
                {
                    Id = 2,
                    Cedula = "0987654321",
                    Nombres = "Test",
                    Apellidos = "Operator",
                    Email = "operator@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                    RolId = 2,
                    Activo = true
                }
            };

            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllUsuarios_ReturnsSuccessResponse()
        {
            var response = await _client.GetAsync("/api/usuario?pagina=1&registrosPorPagina=10");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PagedResponseDto<UsuarioDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Items.Count > 0);
        }

        [Fact]
        public async Task GetUsuarioById_WithExistingId_ReturnsUsuario()
        {
            var response = await _client.GetAsync("/api/usuario/1");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("admin@test.com", result.Data.Email);
        }

        [Fact]
        public async Task GetUsuarioById_WithNonExistentId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/usuario/999");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("Usuario no encontrado", result.Message);
        }

        [Fact]
        public async Task CreateUsuario_WithValidData_ReturnsCreatedUsuario()
        {
            var newUser = new UsuarioCreateDto
            {
                Cedula = "1111111111",
                Nombres = "Nuevo",
                Apellidos = "Usuario",
                Email = "nuevo@test.com",
                Password = "password123",
                RolId = 2
            };

            var json = JsonConvert.SerializeObject(newUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/usuario", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("nuevo@test.com", result.Data.Email);
            Assert.Equal("Usuario creado exitosamente", result.Message);
        }

        [Fact]
        public async Task CreateUsuario_WithExistingEmail_ReturnsBadRequest()
        {
            var duplicateUser = new UsuarioCreateDto
            {
                Cedula = "2222222222",
                Nombres = "Duplicate",
                Apellidos = "User",
                Email = "admin@test.com",
                Password = "password123",
                RolId = 2
            };

            var json = JsonConvert.SerializeObject(duplicateUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/usuario", stringContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("El email ya está registrado", result.Message);
        }

        [Fact]
        public async Task UpdateUsuario_WithValidData_ReturnsUpdatedUsuario()
        {
            var updateUser = new UsuarioUpdateDto
            {
                Id = 2,
                Cedula = "0987654321",
                Nombres = "Updated",
                Apellidos = "Operator",
                Email = "updated.operator@test.com",
                RolId = 2,
                Activo = true
            };

            var json = JsonConvert.SerializeObject(updateUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/api/usuario/2", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Updated", result.Data.Nombres);
            Assert.Equal("updated.operator@test.com", result.Data.Email);
        }

        [Fact]
        public async Task DeleteUsuario_WithExistingId_ReturnsSuccess()
        {
            var response = await _client.DeleteAsync("/api/usuario/2");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<bool>>(content);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Usuario eliminado exitosamente", result.Message);
        }

        [Fact]
        public async Task GetUsuarioByEmail_WithExistingEmail_ReturnsUsuario()
        {
            var response = await _client.GetAsync("/api/usuario/email/admin@test.com");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("admin@test.com", result.Data.Email);
        }

        [Fact]
        public async Task SearchUsuarios_WithValidTerm_ReturnsMatchingUsuarios()
        {
            var response = await _client.GetAsync("/api/usuario/search/admin");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<UsuarioDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count > 0);
            Assert.Contains(result.Data, u => u.Email.Contains("admin"));
        }

        [Fact]
        public async Task GetUsuariosByRol_WithValidRolId_ReturnsUsuariosInRole()
        {
            var response = await _client.GetAsync("/api/usuario/rol/1");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<UsuarioDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count > 0);
            Assert.All(result.Data, u => Assert.Equal(1, u.RolId));
        }

        [Fact]
        public async Task GetTotalActiveUsers_ReturnsCorrectCount()
        {
            var response = await _client.GetAsync("/api/usuario/total-active");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<int>>(content);

            Assert.True(result.Success);
            Assert.True(result.Data >= 2);
        }

        [Fact]
        public async Task CreateUsuario_WithInvalidData_ReturnsBadRequest()
        {
            var invalidUser = new UsuarioCreateDto
            {
                Cedula = "",
                Nombres = "",
                Apellidos = "",
                Email = "invalid-email",
                Password = "",
                RolId = 0
            };

            var json = JsonConvert.SerializeObject(invalidUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/usuario", stringContent);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUsuario_WithMismatchedId_ReturnsBadRequest()
        {
            var updateUser = new UsuarioUpdateDto
            {
                Id = 999,
                Cedula = "0987654321",
                Nombres = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                RolId = 2,
                Activo = true
            };

            var json = JsonConvert.SerializeObject(updateUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/api/usuario/1", stringContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<UsuarioDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("El ID de la URL no coincide con el ID del objeto", result.Message);
        }
    }
}
