using AutoMapper;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Business.Services
{
    public class ArticuloService : IArticuloService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ArticuloService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<PagedResponseDto<ArticuloDto>>> GetAllPagedAsync(ArticuloFiltroDto filtro)
        {
            try
            {
                var totalRegistros = await _unitOfWork.Articulos.CountAsync(a => a.Activo);

                if (filtro.CategoriaId.HasValue)
                    totalRegistros = await _unitOfWork.Articulos.CountAsync(a => a.Activo && a.CategoriaId == filtro.CategoriaId.Value);

                if (filtro.EstadoArticuloId.HasValue)
                    totalRegistros = await _unitOfWork.Articulos.CountAsync(a => a.Activo && a.EstadoArticuloId == filtro.EstadoArticuloId.Value);

                var articulos = await _unitOfWork.Articulos.GetPagedWithFiltersAsync(
                    filtro.Pagina,
                    filtro.RegistrosPorPagina,
                    filtro.CategoriaId,
                    filtro.EstadoArticuloId,
                    filtro.Nombre,
                    filtro.OrdenarPor,
                    !filtro.Descendente);

                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                var pagedResponse = new PagedResponseDto<ArticuloDto>(articulosDto, totalRegistros, filtro.Pagina, filtro.RegistrosPorPagina);

                return new ResponseDto<PagedResponseDto<ArticuloDto>>(pagedResponse);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PagedResponseDto<ArticuloDto>>($"Error al obtener artículos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<ArticuloDto>> GetByIdAsync(int id)
        {
            try
            {
                var articulo = await _unitOfWork.Articulos.GetByIdWithRelationsAsync(id);
                if (articulo == null || !articulo.Activo)
                {
                    return new ResponseDto<ArticuloDto>("Artículo no encontrado");
                }

                var articuloDto = _mapper.Map<ArticuloDto>(articulo);
                return new ResponseDto<ArticuloDto>(articuloDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<ArticuloDto>($"Error al obtener artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<ArticuloDto>> GetByCodigoAsync(string codigo)
        {
            try
            {
                var articulo = await _unitOfWork.Articulos.GetByCodigoAsync(codigo);
                if (articulo == null)
                {
                    return new ResponseDto<ArticuloDto>("Artículo no encontrado");
                }

                var articuloDto = _mapper.Map<ArticuloDto>(articulo);
                return new ResponseDto<ArticuloDto>(articuloDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<ArticuloDto>($"Error al obtener artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<ArticuloDto>> CreateAsync(ArticuloCreateDto articuloCreateDto)
        {
            try
            {
                if (await _unitOfWork.Articulos.ExistsByCodigoAsync(articuloCreateDto.Codigo))
                {
                    return new ResponseDto<ArticuloDto>("Ya existe un artículo con ese código");
                }

                var articulo = _mapper.Map<Articulo>(articuloCreateDto);
                articulo.FechaCreacion = DateTime.Now;
                articulo.FechaActualizacion = DateTime.Now;

                await _unitOfWork.Articulos.AddAsync(articulo);
                await _unitOfWork.SaveChangesAsync();

                var articuloConRelaciones = await _unitOfWork.Articulos.GetByIdWithRelationsAsync(articulo.Id);
                var articuloDto = _mapper.Map<ArticuloDto>(articuloConRelaciones);

                return new ResponseDto<ArticuloDto>(articuloDto, "Artículo creado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<ArticuloDto>($"Error al crear artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<ArticuloDto>> UpdateAsync(ArticuloUpdateDto articuloUpdateDto)
        {
            try
            {
                var articulo = await _unitOfWork.Articulos.GetByIdAsync(articuloUpdateDto.Id);
                if (articulo == null)
                {
                    return new ResponseDto<ArticuloDto>("Artículo no encontrado");
                }

                if (await _unitOfWork.Articulos.ExistsByCodigoAsync(articuloUpdateDto.Codigo, articuloUpdateDto.Id))
                {
                    return new ResponseDto<ArticuloDto>("Ya existe un artículo con ese código");
                }

                articulo.Codigo = articuloUpdateDto.Codigo;
                articulo.Nombre = articuloUpdateDto.Nombre;
                articulo.Descripcion = articuloUpdateDto.Descripcion;
                articulo.Equipo = articuloUpdateDto.Equipo;
                articulo.Temporada = articuloUpdateDto.Temporada;
                articulo.Talla = articuloUpdateDto.Talla;
                articulo.Color = articuloUpdateDto.Color;
                articulo.Precio = articuloUpdateDto.Precio;
                articulo.CategoriaId = articuloUpdateDto.CategoriaId;
                articulo.EstadoArticuloId = articuloUpdateDto.EstadoArticuloId;
                articulo.Ubicacion = articuloUpdateDto.Ubicacion;
                articulo.Stock = articuloUpdateDto.Stock;
                articulo.Activo = articuloUpdateDto.Activo;
                articulo.FechaActualizacion = DateTime.Now;

                _unitOfWork.Articulos.Update(articulo);
                await _unitOfWork.SaveChangesAsync();

                var articuloConRelaciones = await _unitOfWork.Articulos.GetByIdWithRelationsAsync(articulo.Id);
                var articuloDto = _mapper.Map<ArticuloDto>(articuloConRelaciones);

                return new ResponseDto<ArticuloDto>(articuloDto, "Artículo actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<ArticuloDto>($"Error al actualizar artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var articulo = await _unitOfWork.Articulos.GetByIdAsync(id);
                if (articulo == null)
                {
                    return new ResponseDto<bool>("Artículo no encontrado");
                }

                if (!await _unitOfWork.Articulos.CanBeDeletedAsync(id))
                {
                    return new ResponseDto<bool>("No se puede eliminar el artículo porque tiene préstamos activos");
                }

                articulo.Activo = false;
                _unitOfWork.Articulos.Update(articulo);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Artículo eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ArticuloDto>>> GetByCategoriaAsync(int categoriaId)
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.GetByCategoriaAsync(categoriaId);
                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                return new ResponseDto<List<ArticuloDto>>(articulosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ArticuloDto>>($"Error al obtener artículos por categoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ArticuloDto>>> GetByEstadoAsync(int estadoId)
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.GetByEstadoAsync(estadoId);
                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                return new ResponseDto<List<ArticuloDto>>(articulosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ArticuloDto>>($"Error al obtener artículos por estado: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ArticuloDto>>> GetByEquipoAsync(string equipo)
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.GetByEquipoAsync(equipo);
                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                return new ResponseDto<List<ArticuloDto>>(articulosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ArticuloDto>>($"Error al obtener artículos por equipo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ArticuloDto>>> GetAvailableAsync()
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.GetAvailableAsync();
                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                return new ResponseDto<List<ArticuloDto>>(articulosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ArticuloDto>>($"Error al obtener artículos disponibles: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ArticuloDto>>> SearchAsync(string searchTerm)
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.SearchAsync(searchTerm);
                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                return new ResponseDto<List<ArticuloDto>>(articulosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ArticuloDto>>($"Error al buscar artículos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ExistsByCodigoAsync(string codigo)
        {
            try
            {
                var exists = await _unitOfWork.Articulos.ExistsByCodigoAsync(codigo);
                return new ResponseDto<bool>(exists);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al verificar código: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> CanBeDeletedAsync(int id)
        {
            try
            {
                var canBeDeleted = await _unitOfWork.Articulos.CanBeDeletedAsync(id);
                return new ResponseDto<bool>(canBeDeleted);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al verificar eliminación: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetTotalStockAsync()
        {
            try
            {
                var totalStock = await _unitOfWork.Articulos.GetTotalStockAsync();
                return new ResponseDto<int>(totalStock);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al obtener stock total: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetAvailableStockAsync()
        {
            try
            {
                var availableStock = await _unitOfWork.Articulos.GetAvailableStockAsync();
                return new ResponseDto<int>(availableStock);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al obtener stock disponible: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ArticuloDto>>> GetLowStockAsync(int minStock = 1)
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.GetLowStockAsync(minStock);
                var articulosDto = _mapper.Map<List<ArticuloDto>>(articulos);
                return new ResponseDto<List<ArticuloDto>>(articulosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ArticuloDto>>($"Error al obtener artículos con stock bajo: {ex.Message}");
            }
        }
    }
}
