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
    public class PrestamoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPrestamoRepository> _mockPrestamoRepository;
        private readonly Mock<IArticuloRepository> _mockArticuloRepository;
        private readonly Mock<IGenericRepository<EstadoPrestamo>> _mockEstadoPrestamoRepository;
        private readonly Mock<IGenericRepository<EstadoArticulo>> _mockEstadoArticuloRepository;
        private readonly PrestamoService _prestamoService;

        public PrestamoServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockPrestamoRepository = new Mock<IPrestamoRepository>();
            _mockArticuloRepository = new Mock<IArticuloRepository>();
            _mockEstadoPrestamoRepository = new Mock<IGenericRepository<EstadoPrestamo>>();
            _mockEstadoArticuloRepository = new Mock<IGenericRepository<EstadoArticulo>>();

            _mockUnitOfWork.Setup(x => x.Prestamos).Returns(_mockPrestamoRepository.Object);
            _mockUnitOfWork.Setup(x => x.Articulos).Returns(_mockArticuloRepository.Object);
            _mockUnitOfWork.Setup(x => x.Repository<EstadoPrestamo>()).Returns(_mockEstadoPrestamoRepository.Object);
            _mockUnitOfWork.Setup(x => x.Repository<EstadoArticulo>()).Returns(_mockEstadoArticuloRepository.Object);

            _prestamoService = new PrestamoService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsSuccessResponse()
        {
            var createDto = new PrestamoCreateDto
            {
                UsuarioId = 1,
                ArticuloId = 1,
                FechaEntregaEstimada = DateTime.Now.AddDays(7),
                Observaciones = "Préstamo para evento"
            };

            var articulo = new Articulo
            {
                Id = 1,
                Codigo = "PSG-001-L",
                Activo = true,
                EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
            };

            var estadoPendiente = new EstadoPrestamo { Id = 1, Nombre = "Pendiente" };

            var prestamo = new Prestamo
            {
                Id = 1,
                UsuarioId = createDto.UsuarioId,
                ArticuloId = createDto.ArticuloId,
                FechaEntregaEstimada = createDto.FechaEntregaEstimada,
                EstadoPrestamoId = 1
            };

            var prestamoConRelaciones = new Prestamo
            {
                Id = 1,
                UsuarioId = createDto.UsuarioId,
                ArticuloId = createDto.ArticuloId,
                Usuario = new Usuario { Id = 1, Nombres = "Test", Apellidos = "User", Email = "test@test.com" },
                Articulo = new Articulo { Id = 1, Codigo = "PSG-001-L", Nombre = "Camiseta PSG" },
                EstadoPrestamo = estadoPendiente
            };

            var prestamoDto = new PrestamoDto
            {
                Id = 1,
                UsuarioId = createDto.UsuarioId,
                ArticuloId = createDto.ArticuloId,
                UsuarioNombre = "Test User",
                ArticuloNombre = "Camiseta PSG",
                EstadoPrestamoNombre = "Pendiente"
            };

            _mockArticuloRepository.Setup(x => x.GetByIdAsync(createDto.ArticuloId)).ReturnsAsync(articulo);
            _mockPrestamoRepository.Setup(x => x.UserHasActiveLoanAsync(createDto.UsuarioId, createDto.ArticuloId)).ReturnsAsync(false);
            _mockEstadoPrestamoRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EstadoPrestamo, bool>>>())).ReturnsAsync(estadoPendiente);
            _mockMapper.Setup(x => x.Map<Prestamo>(createDto)).Returns(prestamo);
            _mockPrestamoRepository.Setup(x => x.AddAsync(It.IsAny<Prestamo>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockPrestamoRepository.Setup(x => x.GetByIdWithRelationsAsync(It.IsAny<int>())).ReturnsAsync(prestamoConRelaciones);
            _mockMapper.Setup(x => x.Map<PrestamoDto>(prestamoConRelaciones)).Returns(prestamoDto);

            var result = await _prestamoService.CreateAsync(createDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Artículo entregado exitosamente", result.Message);
        }

        [Fact]
        public async Task ReturnAsync_WithValidData_ReturnsSuccessResponse()
        {
            var devolucionDto = new PrestamoDevolucionDto
            {
                Id = 1,
                FechaDevolucionReal = DateTime.Now,
                ObservacionesDevolucion = "Devuelto en buen estado"
            };

            var prestamo = new Prestamo
            {
                Id = 1,
                UsuarioId = 1,
                ArticuloId = 1,
                EstadoPrestamoId = 3
            };

            var articulo = new Articulo
            {
                Id = 1,
                EstadoArticuloId = 2
            };

            var estadoDevuelto = new EstadoPrestamo { Id = 4, Nombre = "Devuelto" };
            var estadoDisponible = new EstadoArticulo { Id = 1, Nombre = "Disponible" };

            var prestamoConRelaciones = new Prestamo
            {
                Id = 1,
                UsuarioId = 1,
                ArticuloId = 1,
                Usuario = new Usuario { Id = 1, Nombres = "Test", Apellidos = "User" },
                Articulo = new Articulo { Id = 1, Codigo = "PSG-001-L", Nombre = "Camiseta PSG" },
                EstadoPrestamo = estadoDevuelto
            };

            var prestamoDto = new PrestamoDto
            {
                Id = 1,
                EstadoPrestamoNombre = "Devuelto"
            };

            _mockPrestamoRepository.Setup(x => x.GetByIdAsync(devolucionDto.Id)).ReturnsAsync(prestamo);
            _mockEstadoPrestamoRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EstadoPrestamo, bool>>>())).ReturnsAsync(estadoDevuelto);
            _mockArticuloRepository.Setup(x => x.GetByIdAsync(prestamo.ArticuloId)).ReturnsAsync(articulo);
            _mockEstadoArticuloRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EstadoArticulo, bool>>>())).ReturnsAsync(estadoDisponible);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockPrestamoRepository.Setup(x => x.GetByIdWithRelationsAsync(devolucionDto.Id)).ReturnsAsync(prestamoConRelaciones);
            _mockMapper.Setup(x => x.Map<PrestamoDto>(prestamoConRelaciones)).Returns(prestamoDto);

            var result = await _prestamoService.ReturnAsync(devolucionDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Artículo devuelto exitosamente", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingPrestamo_ReturnsSuccessResponse()
        {
            var prestamoId = 1;
            var prestamo = new Prestamo
            {
                Id = prestamoId,
                UsuarioId = 1,
                ArticuloId = 1,
                Usuario = new Usuario { Id = 1, Nombres = "Test", Apellidos = "User" },
                Articulo = new Articulo { Id = 1, Codigo = "PSG-001-L", Nombre = "Camiseta PSG" },
                EstadoPrestamo = new EstadoPrestamo { Id = 1, Nombre = "Pendiente" }
            };

            var prestamoDto = new PrestamoDto
            {
                Id = prestamoId,
                UsuarioNombre = "Test User",
                ArticuloNombre = "Camiseta PSG",
                EstadoPrestamoNombre = "Pendiente"
            };

            _mockPrestamoRepository.Setup(x => x.GetByIdWithRelationsAsync(prestamoId)).ReturnsAsync(prestamo);
            _mockMapper.Setup(x => x.Map<PrestamoDto>(prestamo)).Returns(prestamoDto);

            var result = await _prestamoService.GetByIdAsync(prestamoId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(prestamoId, result.Data.Id);
        }

        [Fact]
        public async Task GetByUsuarioAsync_WithValidUserId_ReturnsUserLoans()
        {
            var usuarioId = 1;
            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    UsuarioId = usuarioId,
                    Usuario = new Usuario { Id = usuarioId, Nombres = "Test", Apellidos = "User" },
                    Articulo = new Articulo { Id = 1, Codigo = "PSG-001-L" },
                    EstadoPrestamo = new EstadoPrestamo { Id = 1, Nombre = "Pendiente" }
                }
            };

            var prestamosDto = new List<PrestamoDto>
            {
                new PrestamoDto
                {
                    Id = 1,
                    UsuarioId = usuarioId,
                    UsuarioNombre = "Test User"
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetByUsuarioAsync(usuarioId)).ReturnsAsync(prestamos);
            _mockMapper.Setup(x => x.Map<List<PrestamoDto>>(prestamos)).Returns(prestamosDto);

            var result = await _prestamoService.GetByUsuarioAsync(usuarioId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal(usuarioId, result.Data.First().UsuarioId);
        }

        [Fact]
        public async Task GetPendientesAsync_ReturnsPendingLoans()
        {
            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    Usuario = new Usuario { Id = 1, Nombres = "Test", Apellidos = "User" },
                    Articulo = new Articulo { Id = 1, Codigo = "PSG-001-L" },
                    EstadoPrestamo = new EstadoPrestamo { Id = 1, Nombre = "Pendiente" }
                }
            };

            var prestamosDto = new List<PrestamoDto>
            {
                new PrestamoDto
                {
                    Id = 1,
                    EstadoPrestamoNombre = "Pendiente"
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetPendientesAsync()).ReturnsAsync(prestamos);
            _mockMapper.Setup(x => x.Map<List<PrestamoDto>>(prestamos)).Returns(prestamosDto);

            var result = await _prestamoService.GetPendientesAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Pendiente", result.Data.First().EstadoPrestamoNombre);
        }

        [Fact]
        public async Task GetVencidosAsync_ReturnsOverdueLoans()
        {
            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    FechaDevolucionEstimada = DateTime.Now.AddDays(-5),
                    Usuario = new Usuario { Id = 1, Nombres = "Test", Apellidos = "User" },
                    Articulo = new Articulo { Id = 1, Codigo = "PSG-001-L" },
                    EstadoPrestamo = new EstadoPrestamo { Id = 3, Nombre = "Entregado" }
                }
            };

            var prestamosDto = new List<PrestamoDto>
            {
                new PrestamoDto
                {
                    Id = 1,
                    EstadoPrestamoNombre = "Entregado"
                }
            };

            _mockPrestamoRepository.Setup(x => x.GetVencidosAsync()).ReturnsAsync(prestamos);
            _mockMapper.Setup(x => x.Map<List<PrestamoDto>>(prestamos)).Returns(prestamosDto);

            var result = await _prestamoService.GetVencidosAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task HasActiveLoanAsync_WithActiveLoan_ReturnsTrue()
        {
            var articuloId = 1;

            _mockPrestamoRepository.Setup(x => x.HasActiveLoanAsync(articuloId)).ReturnsAsync(true);

            var result = await _prestamoService.HasActiveLoanAsync(articuloId);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task UserHasActiveLoanAsync_WithUserActiveLoan_ReturnsTrue()
        {
            var usuarioId = 1;
            var articuloId = 1;

            _mockPrestamoRepository.Setup(x => x.UserHasActiveLoanAsync(usuarioId, articuloId)).ReturnsAsync(true);

            var result = await _prestamoService.UserHasActiveLoanAsync(usuarioId, articuloId);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task GetTotalPrestamosAsync_ReturnsCorrectCount()
        {
            var expectedCount = 50;

            _mockPrestamoRepository.Setup(x => x.GetTotalPrestamosAsync()).ReturnsAsync(expectedCount);

            var result = await _prestamoService.GetTotalPrestamosAsync();

            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data);
        }

        [Fact]
        public async Task GetPendientesCountAsync_ReturnsCorrectCount()
        {
            var expectedCount = 5;

            _mockPrestamoRepository.Setup(x => x.GetPendientesCountAsync()).ReturnsAsync(expectedCount);

            var result = await _prestamoService.GetPendientesCountAsync();

            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data);
        }

        [Fact]
        public async Task GetVencidosCountAsync_ReturnsCorrectCount()
        {
            var expectedCount = 3;

            _mockPrestamoRepository.Setup(x => x.GetVencidosCountAsync()).ReturnsAsync(expectedCount);

            var result = await _prestamoService.GetVencidosCountAsync();

            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data);
        }

        [Fact]
        public async Task DeleteAsync_WithPendingLoan_ReturnsSuccessResponse()
        {
            var prestamoId = 1;
            var prestamo = new Prestamo
            {
                Id = prestamoId,
                EstadoPrestamoId = 1
            };

            var estadoPendiente = new EstadoPrestamo { Id = 1, Nombre = "Pendiente" };

            _mockPrestamoRepository.Setup(x => x.GetByIdAsync(prestamoId)).ReturnsAsync(prestamo);
            _mockEstadoPrestamoRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EstadoPrestamo, bool>>>())).ReturnsAsync(estadoPendiente);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _prestamoService.DeleteAsync(prestamoId);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Préstamo eliminado exitosamente", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithNonPendingLoan_ReturnsFailureResponse()
        {
            var prestamoId = 1;
            var prestamo = new Prestamo
            {
                Id = prestamoId,
                EstadoPrestamoId = 2
            };

            var estadoPendiente = new EstadoPrestamo { Id = 1, Nombre = "Pendiente" };

            _mockPrestamoRepository.Setup(x => x.GetByIdAsync(prestamoId)).ReturnsAsync(prestamo);
            _mockEstadoPrestamoRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EstadoPrestamo, bool>>>())).ReturnsAsync(estadoPendiente);

            var result = await _prestamoService.DeleteAsync(prestamoId);

            Assert.False(result.Success);
            Assert.Equal("Solo se pueden eliminar préstamos pendientes", result.Message);
        }
    }
}
