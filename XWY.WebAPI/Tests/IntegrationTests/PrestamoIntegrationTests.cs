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
    public class PrestamoIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PrestamoIntegrationTests(WebApplicationFactory<Program> factory)
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
                        options.UseInMemoryDatabase("TestDb_Prestamo");
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

            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Internacional", Descripcion = "Equipos internacionales", Activo = true },
                new Categoria { Id = 2, Nombre = "Nacional", Descripcion = "Equipos nacionales", Activo = true }
            };

            var estadosArticulo = new List<EstadoArticulo>
            {
                new EstadoArticulo { Id = 1, Nombre = "Disponible", Descripcion = "Disponible para préstamo", Activo = true },
                new EstadoArticulo { Id = 2, Nombre = "Prestado", Descripcion = "Actualmente prestado", Activo = true }
            };

            var estadosPrestamo = new List<EstadoPrestamo>
            {
                new EstadoPrestamo { Id = 1, Nombre = "Pendiente", Descripcion = "Pendiente de aprobación", Activo = true },
                new EstadoPrestamo { Id = 2, Nombre = "Aprobado", Descripcion = "Aprobado", Activo = true },
                new EstadoPrestamo { Id = 3, Nombre = "Entregado", Descripcion = "Entregado al usuario", Activo = true },
                new EstadoPrestamo { Id = 4, Nombre = "Devuelto", Descripcion = "Devuelto correctamente", Activo = true },
                new EstadoPrestamo { Id = 5, Nombre = "Rechazado", Descripcion = "Rechazado", Activo = true }
            };

            context.Roles.AddRange(roles);
            context.Categorias.AddRange(categorias);
            context.EstadosArticulo.AddRange(estadosArticulo);
            context.EstadosPrestamo.AddRange(estadosPrestamo);

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Cedula = "1234567890",
                    Nombres = "Juan",
                    Apellidos = "Pérez",
                    Email = "juan.perez@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    RolId = 2,
                    Activo = true
                },
                new Usuario
                {
                    Id = 2,
                    Cedula = "0987654321",
                    Nombres = "Admin",
                    Apellidos = "User",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RolId = 1,
                    Activo = true
                }
            };

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Equipo = "Paris Saint-Germain",
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Stock = 3,
                    Activo = true
                },
                new Articulo
                {
                    Id = 2,
                    Codigo = "RMA-002-M",
                    Nombre = "Camiseta Real Madrid Local 2024",
                    Equipo = "Real Madrid",
                    CategoriaId = 1,
                    EstadoArticuloId = 2,
                    Stock = 1,
                    Activo = true
                }
            };

            context.Usuarios.AddRange(usuarios);
            context.Articulos.AddRange(articulos);

            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    UsuarioId = 1,
                    ArticuloId = 2,
                    FechaSolicitud = DateTime.Now.AddDays(-10),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-3),
                    FechaEntregaReal = DateTime.Now.AddDays(-2),
                    EstadoPrestamoId = 3,
                    Observaciones = "Préstamo para evento",
                    AprobadoPor = 2,
                    FechaAprobacion = DateTime.Now.AddDays(-5)
                },
                new Prestamo
                {
                    Id = 2,
                    UsuarioId = 1,
                    ArticuloId = 1,
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaEntregaEstimada = DateTime.Now.AddDays(2),
                    EstadoPrestamoId = 1,
                    Observaciones = "Préstamo pendiente"
                }
            };

            context.Prestamos.AddRange(prestamos);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllPrestamos_ReturnsSuccessResponse()
        {
            var response = await _client.GetAsync("/api/prestamo?Pagina=1&RegistrosPorPagina=10");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PagedResponseDto<PrestamoDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Items.Count > 0);
        }

        [Fact]
        public async Task GetPrestamoById_WithExistingId_ReturnsPrestamo()
        {
            var response = await _client.GetAsync("/api/prestamo/1");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Juan Pérez", result.Data.UsuarioNombre);
        }

        [Fact]
        public async Task CreatePrestamo_WithValidData_ReturnsCreatedPrestamo()
        {
            var newPrestamo = new PrestamoCreateDto
            {
                UsuarioId = 1,
                ArticuloId = 1,
                FechaEntregaEstimada = DateTime.Now.AddDays(7),
                FechaDevolucionEstimada = DateTime.Now.AddDays(14),
                Observaciones = "Nuevo préstamo de prueba"
            };

            var json = JsonConvert.SerializeObject(newPrestamo);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.UsuarioId);
            Assert.Equal(1, result.Data.ArticuloId);
            Assert.Equal("Préstamo solicitado exitosamente", result.Message);
        }

        [Fact]
        public async Task CreatePrestamo_WithUnavailableArticle_ReturnsBadRequest()
        {
            var newPrestamo = new PrestamoCreateDto
            {
                UsuarioId = 1,
                ArticuloId = 2,
                FechaEntregaEstimada = DateTime.Now.AddDays(7),
                Observaciones = "Préstamo con artículo no disponible"
            };

            var json = JsonConvert.SerializeObject(newPrestamo);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo", stringContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("El artículo no está disponible", result.Message);
        }

        [Fact]
        public async Task ApprovePrestamo_WithValidData_ReturnsApprovedPrestamo()
        {
            var aprobacionDto = new PrestamoAprobacionDto
            {
                Id = 2,
                Aprobado = true,
                AprobadoPor = 2,
                ObservacionesAprobacion = "Aprobado para el evento"
            };

            var json = JsonConvert.SerializeObject(aprobacionDto);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo/2/approve", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Préstamo aprobado exitosamente", result.Message);
        }

        [Fact]
        public async Task RejectPrestamo_WithValidData_ReturnsRejectedPrestamo()
        {
            var rechazoDto = new PrestamoAprobacionDto
            {
                Id = 2,
                Aprobado = false,
                AprobadoPor = 2,
                ObservacionesAprobacion = "No disponible en este momento"
            };

            var json = JsonConvert.SerializeObject(rechazoDto);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo/2/reject", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Préstamo rechazado exitosamente", result.Message);
        }

        [Fact]
        public async Task DeliverPrestamo_WithValidData_ReturnsDeliveredPrestamo()
        {
            var fechaEntrega = DateTime.Now;
            var json = JsonConvert.SerializeObject(fechaEntrega);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo/1/deliver", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Artículo entregado exitosamente", result.Message);
        }

        [Fact]
        public async Task ReturnPrestamo_WithValidData_ReturnsReturnedPrestamo()
        {
            var devolucionDto = new PrestamoDevolucionDto
            {
                Id = 1,
                FechaDevolucionReal = DateTime.Now,
                ObservacionesDevolucion = "Devuelto en perfectas condiciones"
            };

            var json = JsonConvert.SerializeObject(devolucionDto);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo/1/return", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Artículo devuelto exitosamente", result.Message);
        }

        [Fact]
        public async Task UpdatePrestamo_WithValidData_ReturnsUpdatedPrestamo()
        {
            var updatePrestamo = new PrestamoUpdateDto
            {
                Id = 2,
                FechaEntregaEstimada = DateTime.Now.AddDays(10),
                FechaDevolucionEstimada = DateTime.Now.AddDays(17),
                Observaciones = "Observaciones actualizadas"
            };

            var json = JsonConvert.SerializeObject(updatePrestamo);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/api/prestamo/2", stringContent);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Préstamo actualizado exitosamente", result.Message);
        }

        [Fact]
        public async Task DeletePrestamo_WithPendingLoan_ReturnsSuccess()
        {
            var response = await _client.DeleteAsync("/api/prestamo/2");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<bool>>(content);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Préstamo eliminado exitosamente", result.Message);
        }

        [Fact]
        public async Task GetPrestamosByUsuario_WithValidUserId_ReturnsUserLoans()
        {
            var response = await _client.GetAsync("/api/prestamo/usuario/1");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<PrestamoDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count > 0);
            Assert.All(result.Data, p => Assert.Equal(1, p.UsuarioId));
        }

        [Fact]
        public async Task GetPrestamosByArticulo_WithValidArticleId_ReturnsArticleLoans()
        {
            var response = await _client.GetAsync("/api/prestamo/articulo/2");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<PrestamoDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count > 0);
            Assert.All(result.Data, p => Assert.Equal(2, p.ArticuloId));
        }

        [Fact]
        public async Task GetPrestamosPendientes_ReturnsPendingLoans()
        {
            var response = await _client.GetAsync("/api/prestamo/pendientes");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<PrestamoDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.All(result.Data, p => Assert.Equal("Pendiente", p.EstadoPrestamoNombre));
        }

        [Fact]
        public async Task GetPrestamosActivos_ReturnsActiveLoans()
        {
            var response = await _client.GetAsync("/api/prestamo/activos");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<PrestamoDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetHistorialPrestamos_ReturnsAllLoans()
        {
            var response = await _client.GetAsync("/api/prestamo/historial");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<List<PrestamoDto>>>(content);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Count > 0);
        }

        [Fact]
        public async Task GetTotalPrestamos_ReturnsCorrectCount()
        {
            var response = await _client.GetAsync("/api/prestamo/total");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<int>>(content);

            Assert.True(result.Success);
            Assert.True(result.Data > 0);
        }

        [Fact]
        public async Task GetPendientesCount_ReturnsCorrectCount()
        {
            var response = await _client.GetAsync("/api/prestamo/pendientes-count");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<int>>(content);

            Assert.True(result.Success);
            Assert.True(result.Data >= 0);
        }

        [Fact]
        public async Task GetVencidosCount_ReturnsCorrectCount()
        {
            var response = await _client.GetAsync("/api/prestamo/vencidos-count");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<int>>(content);

            Assert.True(result.Success);
            Assert.True(result.Data >= 0);
        }

        [Fact]
        public async Task GetPrestamoById_WithNonExistentId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/prestamo/999");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("Préstamo no encontrado", result.Message);
        }

        [Fact]
        public async Task CreatePrestamo_WithInvalidData_ReturnsBadRequest()
        {
            var invalidPrestamo = new PrestamoCreateDto
            {
                UsuarioId = 0,
                ArticuloId = 0,
                FechaEntregaEstimada = DateTime.Now.AddDays(-1)
            };

            var json = JsonConvert.SerializeObject(invalidPrestamo);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/prestamo", stringContent);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePrestamo_WithMismatchedId_ReturnsBadRequest()
        {
            var updatePrestamo = new PrestamoUpdateDto
            {
                Id = 999,
                FechaEntregaEstimada = DateTime.Now.AddDays(5),
                Observaciones = "Test"
            };

            var json = JsonConvert.SerializeObject(updatePrestamo);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("/api/prestamo/1", stringContent);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<PrestamoDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("El ID de la URL no coincide con el ID del objeto", result.Message);
        }

    }
}
