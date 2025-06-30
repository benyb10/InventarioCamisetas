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
    public class ReporteServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IArticuloRepository> _mockArticuloRepository;
        private readonly Mock<IPrestamoRepository> _mockPrestamoRepository;
        private readonly ReporteService _reporteService;

        public ReporteServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockArticuloRepository = new Mock<IArticuloRepository>();
            _mockPrestamoRepository = new Mock<IPrestamoRepository>();

            _mockUnitOfWork.Setup(x => x.Articulos).Returns(_mockArticuloRepository.Object);
            _mockUnitOfWork.Setup(x => x.Prestamos).Returns(_mockPrestamoRepository.Object);

            _reporteService = new ReporteService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task ObtenerDatosReporteArticulosAsync_WithValidParameters_ReturnsArticleData()
        {
            var parametros = new ReporteParametrosDto
            {
                CategoriaId = 1,
                EstadoArticuloId = 1
            };

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Equipo = "Paris Saint-Germain",
                    Precio = 89.99m,
                    Stock = 3,
                    Ubicacion = "Estante A-1",
                    Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            _mockArticuloRepository.Setup(x => x.GetFilteredAsync(
                parametros.CategoriaId,
                parametros.EstadoArticuloId,
                null)).ReturnsAsync(articulos);

            var result = await _reporteService.ObtenerDatosReporteArticulosAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("PSG-001-L", result.Data.First().Codigo);
            Assert.Equal("Camiseta PSG Local 2024", result.Data.First().Nombre);
            Assert.Equal("Paris Saint-Germain", result.Data.First().Equipo);
            Assert.Equal("Internacional", result.Data.First().Categoria);
            Assert.Equal("Disponible", result.Data.First().Estado);
        }

        [Fact]
        public async Task ObtenerDatosReportePrestamosAsync_WithValidParameters_ReturnsLoanData()
        {
            var parametros = new ReporteParametrosDto
            {
                EstadoPrestamoId = 1,
                FechaDesde = DateTime.Now.AddDays(-30),
                FechaHasta = DateTime.Now
            };

            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    FechaSolicitud = DateTime.Now.AddDays(-15),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-8),
                    FechaEntregaReal = DateTime.Now.AddDays(-7),
                    Usuario = new Usuario
                    {
                        Id = 1,
                        Nombres = "Juan",
                        Apellidos = "Pérez",
                        Email = "juan.perez@test.com"
                    },
                    Articulo = new Articulo
                    {
                        Id = 1,
                        Codigo = "PSG-001-L",
                        Nombre = "Camiseta PSG Local 2024"
                    },
                    EstadoPrestamo = new EstadoPrestamo { Id = 1, Nombre = "Entregado" },
                    UsuarioAprobador = new Usuario
                    {
                        Id = 2,
                        Nombres = "Admin",
                        Apellidos = "User"
                    }
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetFilteredAsync(
                null,
                null,
                parametros.EstadoPrestamoId,
                parametros.FechaDesde,
                parametros.FechaHasta)).ReturnsAsync(prestamos);

            var result = await _reporteService.ObtenerDatosReportePrestamosAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("PSG-001-L", result.Data.First().CodigoArticulo);
            Assert.Equal("Camiseta PSG Local 2024", result.Data.First().NombreArticulo);
            Assert.Equal("Juan Pérez", result.Data.First().UsuarioNombre);
            Assert.Equal("juan.perez@test.com", result.Data.First().UsuarioEmail);
            Assert.Equal("Entregado", result.Data.First().EstadoPrestamo);
            Assert.Equal("Admin User", result.Data.First().AprobadoPor);
        }

        [Fact]
        public async Task GenerarReporteArticulosPdfAsync_WithValidData_ReturnsPdfBytes()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "articulos",
                Formato = "pdf"
            };

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Equipo = "Paris Saint-Germain",
                    Precio = 89.99m,
                    Stock = 3,
                    Ubicacion = "Estante A-1",
                    Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            _mockArticuloRepository.Setup(x => x.GetFilteredAsync(
                parametros.CategoriaId,
                parametros.EstadoArticuloId,
                null)).ReturnsAsync(articulos);

            var result = await _reporteService.GenerarReporteArticulosPdfAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
            Assert.Equal("Reporte PDF generado exitosamente", result.Message);
        }

        [Fact]
        public async Task GenerarReporteArticulosExcelAsync_WithValidData_ReturnsExcelBytes()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "articulos",
                Formato = "excel"
            };

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "RMA-002-M",
                    Nombre = "Camiseta Real Madrid Local 2024",
                    Equipo = "Real Madrid",
                    Precio = 94.99m,
                    Stock = 2,
                    Ubicacion = "Estante A-2",
                    Categoria = new Categoria { Id = 2, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            _mockArticuloRepository.Setup(x => x.GetFilteredAsync(
                parametros.CategoriaId,
                parametros.EstadoArticuloId,
                null)).ReturnsAsync(articulos);

            var result = await _reporteService.GenerarReporteArticulosExcelAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
            Assert.Equal("Reporte Excel generado exitosamente", result.Message);
        }

        [Fact]
        public async Task GenerarReportePrestamosPdfAsync_WithValidData_ReturnsPdfBytes()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "prestamos",
                Formato = "pdf"
            };

            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    FechaSolicitud = DateTime.Now.AddDays(-10),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-3),
                    Usuario = new Usuario
                    {
                        Id = 1,
                        Nombres = "María",
                        Apellidos = "González",
                        Email = "maria.gonzalez@test.com"
                    },
                    Articulo = new Articulo
                    {
                        Id = 1,
                        Codigo = "BAR-003-XL",
                        Nombre = "Camiseta FC Barcelona Local 2024"
                    },
                    EstadoPrestamo = new EstadoPrestamo { Id = 2, Nombre = "Aprobado" }
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetFilteredAsync(
                null,
                null,
                parametros.EstadoPrestamoId,
                parametros.FechaDesde,
                parametros.FechaHasta)).ReturnsAsync(prestamos);

            var result = await _reporteService.GenerarReportePrestamosPdfAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
            Assert.Equal("Reporte PDF generado exitosamente", result.Message);
        }

        [Fact]
        public async Task GenerarReportePrestamosExcelAsync_WithValidData_ReturnsExcelBytes()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "prestamos",
                Formato = "excel"
            };

            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaEntregaReal = DateTime.Now.AddDays(-2),
                    FechaDevolucionReal = DateTime.Now.AddDays(-1),
                    Usuario = new Usuario
                    {
                        Id = 1,
                        Nombres = "Carlos",
                        Apellidos = "Ruiz",
                        Email = "carlos.ruiz@test.com"
                    },
                    Articulo = new Articulo
                    {
                        Id = 1,
                        Codigo = "MCI-004-L",
                        Nombre = "Camiseta Manchester City Local 2024"
                    },
                    EstadoPrestamo = new EstadoPrestamo { Id = 4, Nombre = "Devuelto" }
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetFilteredAsync(
                null,
                null,
                parametros.EstadoPrestamoId,
                parametros.FechaDesde,
                parametros.FechaHasta)).ReturnsAsync(prestamos);

            var result = await _reporteService.GenerarReportePrestamosExcelAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
            Assert.Equal("Reporte Excel generado exitosamente", result.Message);
        }

        [Fact]
        public async Task GenerarReportePersonalizadoAsync_WithArticlesPdf_CallsCorrectMethod()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "articulos",
                Formato = "pdf"
            };

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "ECU-005-M",
                    Nombre = "Camiseta Selección Ecuador 2024",
                    Equipo = "Selección Ecuador",
                    Categoria = new Categoria { Id = 3, Nombre = "Selecciones" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            _mockArticuloRepository.Setup(x => x.GetFilteredAsync(
                parametros.CategoriaId,
                parametros.EstadoArticuloId,
                null)).ReturnsAsync(articulos);

            var result = await _reporteService.GenerarReportePersonalizadoAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
        }

        [Fact]
        public async Task GenerarReportePersonalizadoAsync_WithLoansPdf_CallsCorrectMethod()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "prestamos",
                Formato = "pdf"
            };

            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    Usuario = new Usuario { Nombres = "Test", Apellidos = "User", Email = "test@test.com" },
                    Articulo = new Articulo { Codigo = "ECU-005-M", Nombre = "Camiseta Ecuador" },
                    EstadoPrestamo = new EstadoPrestamo { Nombre = "Pendiente" }
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetFilteredAsync(
                null,
                null,
                parametros.EstadoPrestamoId,
                parametros.FechaDesde,
                parametros.FechaHasta)).ReturnsAsync(prestamos);

            var result = await _reporteService.GenerarReportePersonalizadoAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
        }

        [Fact]
        public async Task GenerarReportePersonalizadoAsync_WithInvalidType_ReturnsFailure()
        {
            var parametros = new ReporteParametrosDto
            {
                TipoReporte = "invalid",
                Formato = "pdf"
            };

            var result = await _reporteService.GenerarReportePersonalizadoAsync(parametros);

            Assert.False(result.Success);
            Assert.Equal("Tipo de reporte no válido", result.Message);
        }

        [Fact]
        public async Task ObtenerDatosReporteArticulosAsync_WithEmptyData_ReturnsEmptyList()
        {
            var parametros = new ReporteParametrosDto();
            var articulos = new List<Articulo>();

            _mockArticuloRepository.Setup(x => x.GetFilteredAsync(
                parametros.CategoriaId,
                parametros.EstadoArticuloId,
                null)).ReturnsAsync(articulos);

            var result = await _reporteService.ObtenerDatosReporteArticulosAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task ObtenerDatosReportePrestamosAsync_WithEmptyData_ReturnsEmptyList()
        {
            var parametros = new ReporteParametrosDto();
            var prestamos = new List<Prestamo>();

            _mockPrestamoRepository.Setup(x => x.GetFilteredAsync(
                null,
                null,
                parametros.EstadoPrestamoId,
                parametros.FechaDesde,
                parametros.FechaHasta)).ReturnsAsync(prestamos);

            var result = await _reporteService.ObtenerDatosReportePrestamosAsync(parametros);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }
    }
}
