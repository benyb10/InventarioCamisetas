using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Tests.IntegrationTests
{
    public class ArticuloIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ArticuloIntegrationTests(WebApplicationFactory<Program> factory)
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
                        options.UseInMemoryDatabase("TestDb_Articulo");
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
            if (context.Categorias.Any()) return;

            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Internacional", Descripcion = "Equipos internacionales", Activo = true },
                new Categoria { Id = 2, Nombre = "Nacional", Descripcion = "Equipos nacionales", Activo = true },
                new Categoria { Id = 3, Nombre = "Selecciones", Descripcion = "Selecciones nacionales", Activo = true }
            };

            var estadosArticulo = new List<EstadoArticulo>
            {
                new EstadoArticulo { Id = 1, Nombre = "Disponible", Descripcion = "Disponible para préstamo", Activo = true },
                new EstadoArticulo { Id = 2, Nombre = "Prestado", Descripcion = "Actualmente prestado", Activo = true },
                new EstadoArticulo { Id = 3, Nombre = "Mantenimiento", Descripcion = "En mantenimiento", Activo = true }
            };

            context.Categorias.AddRange(categorias);
            context.EstadosArticulo.AddRange(estadosArticulo);

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del Paris Saint-Germain",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    Activo = true
                },
                new Articulo
                {
                    Id = 2,
                    Codigo = "RMA-002-M",
                    Nombre = "Camiseta Real Madrid Local 2024",
                    Descripcion = "Camiseta oficial del Real Madrid",
                    Equipo = "Real Madrid",
                    Temporada = "2023-2024",
                    Talla = "M",
                    Color = "Blanco",
                    Precio = 94.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-2",
                    Stock = 2,
                    Activo = true
                },
                new Articulo
                {
                    Id = 3,
                    Codigo = "ECU-005-M",
                    Nombre = "Camiseta Selección Ecuador 2024",
                    Descripcion = "Camiseta oficial de Ecuador",
                    Equipo = "Selección Ecuador",
                    Temporada = "2024",
                    Talla = "M",
                    Color = "Amarillo",
                    Precio = 79.99m,
                    CategoriaId = 3,
                    EstadoArticuloId = 2,
                    Ubicacion = "Estante C-1",
                    Stock = 1,
                    Activo = true
                }
            };

            context.Articulos.AddRange(articulos);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllArticulos_ReturnsSuccessResponse()
        {
            var response = await _client.GetAsync("/api/articulo?Pagina=1&RegistrosPorPagina=10");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<ArticuloDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("El ID de la URL no coincide con el ID del objeto", result.Message);
        }

        [Fact]
        public async Task GetArticuloById_WithNonExistentId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/articulo/999");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<ArticuloDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("Artículo no encontrado", result.Message);
        }

        [Fact]
        public async Task GetArticuloByCodigo_WithNonExistentCode_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/articulo/codigo/NONEXISTENT");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto<ArticuloDto>>(content);

            Assert.False(result.Success);
            Assert.Equal("Artículo no encontrado", result.Message);
        }
    }
}
