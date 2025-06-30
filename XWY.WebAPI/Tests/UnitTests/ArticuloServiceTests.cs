using Xunit;
using Moq;
using AutoMapper;
using XWY.WebAPI.Business.Services;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;
using XWY.WebAPI.DataAccess.Repositories;

namespace XWY.WebAPI.Tests.UnitTests
{
    public class ArticuloServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IArticuloRepository> _mockArticuloRepository;
        private readonly ArticuloService _articuloService;

        public ArticuloServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockArticuloRepository = new Mock<IArticuloRepository>();

            _mockUnitOfWork.Setup(x => x.Articulos).Returns(_mockArticuloRepository.Object);

            _articuloService = new ArticuloService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingArticle_ReturnsSuccessResponse()
        {
            var articuloId = 1;
            var articulo = new Articulo
            {
                Id = articuloId,
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
                Activo = true,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
            };

            var articuloDto = new ArticuloDto
            {
                Id = articuloId,
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
                CategoriaNombre = "Internacional",
                EstadoArticuloNombre = "Disponible",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                Activo = true
            };

            _mockArticuloRepository.Setup(x => x.GetByIdWithRelationsAsync(articuloId)).ReturnsAsync(articulo);
            _mockMapper.Setup(x => x.Map<ArticuloDto>(articulo)).Returns(articuloDto);

            var result = await _articuloService.GetByIdAsync(articuloId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(articuloId, result.Data.Id);
            Assert.Equal("PSG-001-L", result.Data.Codigo);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentArticle_ReturnsFailureResponse()
        {
            var articuloId = 999;

            _mockArticuloRepository.Setup(x => x.GetByIdWithRelationsAsync(articuloId)).ReturnsAsync((Articulo)null);

            var result = await _articuloService.GetByIdAsync(articuloId);

            Assert.False(result.Success);
            Assert.Equal("Artículo no encontrado", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsSuccessResponse()
        {
            var createDto = new ArticuloCreateDto
            {
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
                Stock = 5
            };

            var articulo = new Articulo
            {
                Id = 1,
                Codigo = createDto.Codigo,
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion,
                Equipo = createDto.Equipo,
                Temporada = createDto.Temporada,
                Talla = createDto.Talla,
                Color = createDto.Color,
                Precio = createDto.Precio,
                CategoriaId = createDto.CategoriaId,
                EstadoArticuloId = createDto.EstadoArticuloId,
                Ubicacion = createDto.Ubicacion,
                Stock = createDto.Stock
            };

            var articuloConRelaciones = new Articulo
            {
                Id = 1,
                Codigo = createDto.Codigo,
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion,
                Equipo = createDto.Equipo,
                Temporada = createDto.Temporada,
                Talla = createDto.Talla,
                Color = createDto.Color,
                Precio = createDto.Precio,
                CategoriaId = createDto.CategoriaId,
                EstadoArticuloId = createDto.EstadoArticuloId,
                Ubicacion = createDto.Ubicacion,
                Stock = createDto.Stock,
                Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
            };

            var articuloDto = new ArticuloDto
            {
                Id = 1,
                Codigo = createDto.Codigo,
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion,
                Equipo = createDto.Equipo,
                Temporada = createDto.Temporada,
                Talla = createDto.Talla,
                Color = createDto.Color,
                Precio = createDto.Precio,
                CategoriaId = createDto.CategoriaId,
                EstadoArticuloId = createDto.EstadoArticuloId,
                Ubicacion = createDto.Ubicacion,
                Stock = createDto.Stock,
                CategoriaNombre = "Internacional",
                EstadoArticuloNombre = "Disponible",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                Activo = true
            };

            _mockArticuloRepository.Setup(x => x.ExistsByCodigoAsync(createDto.Codigo)).ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<Articulo>(createDto)).Returns(articulo);
            _mockArticuloRepository.Setup(x => x.AddAsync(It.IsAny<Articulo>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockArticuloRepository.Setup(x => x.GetByIdWithRelationsAsync(It.IsAny<int>())).ReturnsAsync(articuloConRelaciones);
            _mockMapper.Setup(x => x.Map<ArticuloDto>(articuloConRelaciones)).Returns(articuloDto);

            var result = await _articuloService.CreateAsync(createDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(createDto.Codigo, result.Data.Codigo);
            Assert.Equal("Artículo creado exitosamente", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithExistingCode_ReturnsFailureResponse()
        {
            var createDto = new ArticuloCreateDto
            {
                Codigo = "PSG-001-L"
            };

            _mockArticuloRepository.Setup(x => x.ExistsByCodigoAsync(createDto.Codigo)).ReturnsAsync(true);

            var result = await _articuloService.CreateAsync(createDto);

            Assert.False(result.Success);
            Assert.Equal("Ya existe un artículo con ese código", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ReturnsSuccessResponse()
        {
            var updateDto = new ArticuloUpdateDto
            {
                Id = 1,
                Codigo = "PSG-001-L-UPD",
                Nombre = "Camiseta PSG Local 2024 Updated",
                Descripcion = "Camiseta oficial actualizada",
                Equipo = "Paris Saint-Germain",
                Temporada = "2023-2024",
                Talla = "L",
                Color = "Azul Marino",
                Precio = 99.99m,
                CategoriaId = 1,
                EstadoArticuloId = 1,
                Ubicacion = "Estante A-1",
                Stock = 3,
                Activo = true
            };

            var existingArticulo = new Articulo
            {
                Id = 1,
                Codigo = "PSG-001-L",
                Nombre = "Camiseta PSG Local 2024",
                Equipo = "Paris Saint-Germain"
            };

            var updatedArticulo = new Articulo
            {
                Id = 1,
                Codigo = updateDto.Codigo,
                Nombre = updateDto.Nombre,
                Descripcion = updateDto.Descripcion,
                Equipo = updateDto.Equipo,
                Temporada = updateDto.Temporada,
                Talla = updateDto.Talla,
                Color = updateDto.Color,
                Precio = updateDto.Precio,
                CategoriaId = updateDto.CategoriaId,
                EstadoArticuloId = updateDto.EstadoArticuloId,
                Ubicacion = updateDto.Ubicacion,
                Stock = updateDto.Stock,
                Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
            };

            var articuloDto = new ArticuloDto
            {
                Id = 1,
                Codigo = updateDto.Codigo,
                Nombre = updateDto.Nombre,
                Descripcion = updateDto.Descripcion,
                Equipo = updateDto.Equipo,
                Temporada = updateDto.Temporada,
                Talla = updateDto.Talla,
                Color = updateDto.Color,
                Precio = updateDto.Precio,
                CategoriaId = updateDto.CategoriaId,
                EstadoArticuloId = updateDto.EstadoArticuloId,
                Ubicacion = updateDto.Ubicacion,
                Stock = updateDto.Stock,
                CategoriaNombre = "Internacional",
                EstadoArticuloNombre = "Disponible",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                Activo = updateDto.Activo
            };

            _mockArticuloRepository.Setup(x => x.GetByIdAsync(updateDto.Id)).ReturnsAsync(existingArticulo);
            _mockArticuloRepository.Setup(x => x.ExistsByCodigoAsync(updateDto.Codigo, updateDto.Id)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mockArticuloRepository.Setup(x => x.GetByIdWithRelationsAsync(updateDto.Id)).ReturnsAsync(updatedArticulo);
            _mockMapper.Setup(x => x.Map<ArticuloDto>(updatedArticulo)).Returns(articuloDto);

            var result = await _articuloService.UpdateAsync(updateDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(updateDto.Codigo, result.Data.Codigo);
            Assert.Equal("Artículo actualizado exitosamente", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithValidArticle_ReturnsSuccessResponse()
        {
            var articuloId = 1;
            var articulo = new Articulo
            {
                Id = articuloId,
                Codigo = "PSG-001-L",
                Activo = true
            };

            _mockArticuloRepository.Setup(x => x.GetByIdAsync(articuloId)).ReturnsAsync(articulo);
            _mockArticuloRepository.Setup(x => x.CanBeDeletedAsync(articuloId)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _articuloService.DeleteAsync(articuloId);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Artículo eliminado exitosamente", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithActiveLoans_ReturnsFailureResponse()
        {
            var articuloId = 1;
            var articulo = new Articulo
            {
                Id = articuloId,
                Codigo = "PSG-001-L",
                Activo = true
            };

            _mockArticuloRepository.Setup(x => x.GetByIdAsync(articuloId)).ReturnsAsync(articulo);
            _mockArticuloRepository.Setup(x => x.CanBeDeletedAsync(articuloId)).ReturnsAsync(false);

            var result = await _articuloService.DeleteAsync(articuloId);

            Assert.False(result.Success);
            Assert.Equal("No se puede eliminar el artículo porque tiene préstamos activos", result.Message);
        }

        [Fact]
        public async Task GetAvailableAsync_ReturnsAvailableArticles()
        {
            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            var articulosDto = new List<ArticuloDto>
            {
                new ArticuloDto
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    CategoriaNombre = "Internacional",
                    EstadoArticuloNombre = "Disponible",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Activo = true
                }
            };

            _mockArticuloRepository.Setup(x => x.GetAvailableAsync()).ReturnsAsync(articulos);
            _mockMapper.Setup(x => x.Map<List<ArticuloDto>>(articulos)).Returns(articulosDto);

            var result = await _articuloService.GetAvailableAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Disponible", result.Data.First().EstadoArticuloNombre);
        }

        [Fact]
        public async Task SearchAsync_WithValidTerm_ReturnsMatchingArticles()
        {
            var searchTerm = "PSG";
            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            var articulosDto = new List<ArticuloDto>
            {
                new ArticuloDto
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    CategoriaNombre = "Internacional",
                    EstadoArticuloNombre = "Disponible",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Activo = true
                }
            };

            _mockArticuloRepository.Setup(x => x.SearchAsync(searchTerm)).ReturnsAsync(articulos);
            _mockMapper.Setup(x => x.Map<List<ArticuloDto>>(articulos)).Returns(articulosDto);

            var result = await _articuloService.SearchAsync(searchTerm);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Contains("PSG", result.Data.First().Codigo);
        }

        [Fact]
        public async Task GetByCategoriaAsync_WithValidCategoryId_ReturnsArticlesInCategory()
        {
            var categoriaId = 1;
            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = categoriaId,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    Categoria = new Categoria { Id = categoriaId, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            var articulosDto = new List<ArticuloDto>
            {
                new ArticuloDto
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = categoriaId,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    CategoriaNombre = "Internacional",
                    EstadoArticuloNombre = "Disponible",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Activo = true
                }
            };

            _mockArticuloRepository.Setup(x => x.GetByCategoriaAsync(categoriaId)).ReturnsAsync(articulos);
            _mockMapper.Setup(x => x.Map<List<ArticuloDto>>(articulos)).Returns(articulosDto);

            var result = await _articuloService.GetByCategoriaAsync(categoriaId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal(categoriaId, result.Data.First().CategoriaId);
        }

        [Fact]
        public async Task GetTotalStockAsync_ReturnsCorrectTotal()
        {
            var expectedStock = 150;

            _mockArticuloRepository.Setup(x => x.GetTotalStockAsync()).ReturnsAsync(expectedStock);

            var result = await _articuloService.GetTotalStockAsync();

            Assert.True(result.Success);
            Assert.Equal(expectedStock, result.Data);
        }

        [Fact]
        public async Task GetAvailableStockAsync_ReturnsCorrectAvailableStock()
        {
            var expectedAvailableStock = 120;

            _mockArticuloRepository.Setup(x => x.GetAvailableStockAsync()).ReturnsAsync(expectedAvailableStock);

            var result = await _articuloService.GetAvailableStockAsync();

            Assert.True(result.Success);
            Assert.Equal(expectedAvailableStock, result.Data);
        }

        [Fact]
        public async Task GetLowStockAsync_ReturnsArticlesWithLowStock()
        {
            var minStock = 2;
            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 1,
                    Categoria = new Categoria { Id = 1, Nombre = "Internacional" },
                    EstadoArticulo = new EstadoArticulo { Id = 1, Nombre = "Disponible" }
                }
            };

            var articulosDto = new List<ArticuloDto>
            {
                new ArticuloDto
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del PSG",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 1,
                    CategoriaNombre = "Internacional",
                    EstadoArticuloNombre = "Disponible",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Activo = true
                }
            };

            _mockArticuloRepository.Setup(x => x.GetLowStockAsync(minStock)).ReturnsAsync(articulos);
            _mockMapper.Setup(x => x.Map<List<ArticuloDto>>(articulos)).Returns(articulosDto);

            var result = await _articuloService.GetLowStockAsync(minStock);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.True(result.Data.First().Stock <= minStock);
        }

        [Fact]
        public async Task ExistsByCodigoAsync_WithExistingCode_ReturnsTrue()
        {
            var codigo = "PSG-001-L";

            _mockArticuloRepository.Setup(x => x.ExistsByCodigoAsync(codigo)).ReturnsAsync(true);

            var result = await _articuloService.ExistsByCodigoAsync(codigo);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task CanBeDeletedAsync_WithDeletableArticle_ReturnsTrue()
        {
            var articuloId = 1;

            _mockArticuloRepository.Setup(x => x.CanBeDeletedAsync(articuloId)).ReturnsAsync(true);

            var result = await _articuloService.CanBeDeletedAsync(articuloId);

            Assert.True(result.Success);
            Assert.True(result.Data);
        }
    }
}