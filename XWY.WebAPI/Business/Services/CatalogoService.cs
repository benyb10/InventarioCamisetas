using AutoMapper;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Business.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CatalogoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<List<RolDto>>> GetRolesAsync()
        {
            try
            {
                var roles = await _unitOfWork.Repository<Rol>().FindAsync(r => r.Activo);
                var rolesDto = _mapper.Map<List<RolDto>>(roles);
                return new ResponseDto<List<RolDto>>(rolesDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<RolDto>>($"Error al obtener roles: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<CategoriaDto>>> GetCategoriasAsync()
        {
            try
            {
                var categorias = await _unitOfWork.Repository<Categoria>().FindAsync(c => c.Activo);
                var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);
                return new ResponseDto<List<CategoriaDto>>(categoriasDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<CategoriaDto>>($"Error al obtener categorías: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<EstadoArticuloDto>>> GetEstadosArticuloAsync()
        {
            try
            {
                var estados = await _unitOfWork.Repository<EstadoArticulo>().FindAsync(e => e.Activo);
                var estadosDto = _mapper.Map<List<EstadoArticuloDto>>(estados);
                return new ResponseDto<List<EstadoArticuloDto>>(estadosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<EstadoArticuloDto>>($"Error al obtener estados de artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<EstadoPrestamoDto>>> GetEstadosPrestamoAsync()
        {
            try
            {
                var estados = await _unitOfWork.Repository<EstadoPrestamo>().FindAsync(e => e.Activo);
                var estadosDto = _mapper.Map<List<EstadoPrestamoDto>>(estados);
                return new ResponseDto<List<EstadoPrestamoDto>>(estadosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<EstadoPrestamoDto>>($"Error al obtener estados de préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<RolDto>> CreateRolAsync(RolCreateDto rolCreateDto)
        {
            try
            {
                var existeRol = await _unitOfWork.Repository<Rol>()
                    .ExistsAsync(r => r.Nombre == rolCreateDto.Nombre);

                if (existeRol)
                {
                    return new ResponseDto<RolDto>("Ya existe un rol con ese nombre");
                }

                var rol = _mapper.Map<Rol>(rolCreateDto);
                rol.FechaCreacion = DateTime.Now;

                await _unitOfWork.Repository<Rol>().AddAsync(rol);
                await _unitOfWork.SaveChangesAsync();

                var rolDto = _mapper.Map<RolDto>(rol);
                return new ResponseDto<RolDto>(rolDto, "Rol creado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<RolDto>($"Error al crear rol: {ex.Message}");
            }
        }

        public async Task<ResponseDto<CategoriaDto>> CreateCategoriaAsync(CategoriaCreateDto categoriaCreateDto)
        {
            try
            {
                var existeCategoria = await _unitOfWork.Repository<Categoria>()
                    .ExistsAsync(c => c.Nombre == categoriaCreateDto.Nombre);

                if (existeCategoria)
                {
                    return new ResponseDto<CategoriaDto>("Ya existe una categoría con ese nombre");
                }

                var categoria = _mapper.Map<Categoria>(categoriaCreateDto);
                categoria.FechaCreacion = DateTime.Now;

                await _unitOfWork.Repository<Categoria>().AddAsync(categoria);
                await _unitOfWork.SaveChangesAsync();

                var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
                return new ResponseDto<CategoriaDto>(categoriaDto, "Categoría creada exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<CategoriaDto>($"Error al crear categoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<EstadoArticuloDto>> CreateEstadoArticuloAsync(EstadoArticuloCreateDto estadoCreateDto)
        {
            try
            {
                var existeEstado = await _unitOfWork.Repository<EstadoArticulo>()
                    .ExistsAsync(e => e.Nombre == estadoCreateDto.Nombre);

                if (existeEstado)
                {
                    return new ResponseDto<EstadoArticuloDto>("Ya existe un estado con ese nombre");
                }

                var estado = _mapper.Map<EstadoArticulo>(estadoCreateDto);
                estado.FechaCreacion = DateTime.Now;

                await _unitOfWork.Repository<EstadoArticulo>().AddAsync(estado);
                await _unitOfWork.SaveChangesAsync();

                var estadoDto = _mapper.Map<EstadoArticuloDto>(estado);
                return new ResponseDto<EstadoArticuloDto>(estadoDto, "Estado de artículo creado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<EstadoArticuloDto>($"Error al crear estado de artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<EstadoPrestamoDto>> CreateEstadoPrestamoAsync(EstadoPrestamoCreateDto estadoCreateDto)
        {
            try
            {
                var existeEstado = await _unitOfWork.Repository<EstadoPrestamo>()
                    .ExistsAsync(e => e.Nombre == estadoCreateDto.Nombre);

                if (existeEstado)
                {
                    return new ResponseDto<EstadoPrestamoDto>("Ya existe un estado con ese nombre");
                }

                var estado = _mapper.Map<EstadoPrestamo>(estadoCreateDto);
                estado.FechaCreacion = DateTime.Now;

                await _unitOfWork.Repository<EstadoPrestamo>().AddAsync(estado);
                await _unitOfWork.SaveChangesAsync();

                var estadoDto = _mapper.Map<EstadoPrestamoDto>(estado);
                return new ResponseDto<EstadoPrestamoDto>(estadoDto, "Estado de préstamo creado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<EstadoPrestamoDto>($"Error al crear estado de préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<RolDto>> UpdateRolAsync(RolUpdateDto rolUpdateDto)
        {
            try
            {
                var rol = await _unitOfWork.Repository<Rol>().GetByIdAsync(rolUpdateDto.Id);
                if (rol == null)
                {
                    return new ResponseDto<RolDto>("Rol no encontrado");
                }

                var existeRol = await _unitOfWork.Repository<Rol>()
                    .ExistsAsync(r => r.Nombre == rolUpdateDto.Nombre && r.Id != rolUpdateDto.Id);

                if (existeRol)
                {
                    return new ResponseDto<RolDto>("Ya existe un rol con ese nombre");
                }

                rol.Nombre = rolUpdateDto.Nombre;
                rol.Descripcion = rolUpdateDto.Descripcion;
                rol.Activo = rolUpdateDto.Activo;

                _unitOfWork.Repository<Rol>().Update(rol);
                await _unitOfWork.SaveChangesAsync();

                var rolDto = _mapper.Map<RolDto>(rol);
                return new ResponseDto<RolDto>(rolDto, "Rol actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<RolDto>($"Error al actualizar rol: {ex.Message}");
            }
        }

        public async Task<ResponseDto<CategoriaDto>> UpdateCategoriaAsync(CategoriaUpdateDto categoriaUpdateDto)
        {
            try
            {
                var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(categoriaUpdateDto.Id);
                if (categoria == null)
                {
                    return new ResponseDto<CategoriaDto>("Categoría no encontrada");
                }

                var existeCategoria = await _unitOfWork.Repository<Categoria>()
                    .ExistsAsync(c => c.Nombre == categoriaUpdateDto.Nombre && c.Id != categoriaUpdateDto.Id);

                if (existeCategoria)
                {
                    return new ResponseDto<CategoriaDto>("Ya existe una categoría con ese nombre");
                }

                categoria.Nombre = categoriaUpdateDto.Nombre;
                categoria.Descripcion = categoriaUpdateDto.Descripcion;
                categoria.Activo = categoriaUpdateDto.Activo;

                _unitOfWork.Repository<Categoria>().Update(categoria);
                await _unitOfWork.SaveChangesAsync();

                var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
                return new ResponseDto<CategoriaDto>(categoriaDto, "Categoría actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<CategoriaDto>($"Error al actualizar categoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<EstadoArticuloDto>> UpdateEstadoArticuloAsync(EstadoArticuloUpdateDto estadoUpdateDto)
        {
            try
            {
                var estado = await _unitOfWork.Repository<EstadoArticulo>().GetByIdAsync(estadoUpdateDto.Id);
                if (estado == null)
                {
                    return new ResponseDto<EstadoArticuloDto>("Estado de artículo no encontrado");
                }

                var existeEstado = await _unitOfWork.Repository<EstadoArticulo>()
                    .ExistsAsync(e => e.Nombre == estadoUpdateDto.Nombre && e.Id != estadoUpdateDto.Id);

                if (existeEstado)
                {
                    return new ResponseDto<EstadoArticuloDto>("Ya existe un estado con ese nombre");
                }

                estado.Nombre = estadoUpdateDto.Nombre;
                estado.Descripcion = estadoUpdateDto.Descripcion;
                estado.Activo = estadoUpdateDto.Activo;

                _unitOfWork.Repository<EstadoArticulo>().Update(estado);
                await _unitOfWork.SaveChangesAsync();

                var estadoDto = _mapper.Map<EstadoArticuloDto>(estado);
                return new ResponseDto<EstadoArticuloDto>(estadoDto, "Estado de artículo actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<EstadoArticuloDto>($"Error al actualizar estado de artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<EstadoPrestamoDto>> UpdateEstadoPrestamoAsync(EstadoPrestamoUpdateDto estadoUpdateDto)
        {
            try
            {
                var estado = await _unitOfWork.Repository<EstadoPrestamo>().GetByIdAsync(estadoUpdateDto.Id);
                if (estado == null)
                {
                    return new ResponseDto<EstadoPrestamoDto>("Estado de préstamo no encontrado");
                }

                var existeEstado = await _unitOfWork.Repository<EstadoPrestamo>()
                    .ExistsAsync(e => e.Nombre == estadoUpdateDto.Nombre && e.Id != estadoUpdateDto.Id);

                if (existeEstado)
                {
                    return new ResponseDto<EstadoPrestamoDto>("Ya existe un estado con ese nombre");
                }

                estado.Nombre = estadoUpdateDto.Nombre;
                estado.Descripcion = estadoUpdateDto.Descripcion;
                estado.Activo = estadoUpdateDto.Activo;

                _unitOfWork.Repository<EstadoPrestamo>().Update(estado);
                await _unitOfWork.SaveChangesAsync();

                var estadoDto = _mapper.Map<EstadoPrestamoDto>(estado);
                return new ResponseDto<EstadoPrestamoDto>(estadoDto, "Estado de préstamo actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<EstadoPrestamoDto>($"Error al actualizar estado de préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteRolAsync(int id)
        {
            try
            {
                var rol = await _unitOfWork.Repository<Rol>().GetByIdAsync(id);
                if (rol == null)
                {
                    return new ResponseDto<bool>("Rol no encontrado");
                }

                rol.Activo = false;
                _unitOfWork.Repository<Rol>().Update(rol);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Rol eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar rol: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteCategoriaAsync(int id)
        {
            try
            {
                var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id);
                if (categoria == null)
                {
                    return new ResponseDto<bool>("Categoría no encontrada");
                }

                categoria.Activo = false;
                _unitOfWork.Repository<Categoria>().Update(categoria);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Categoría eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar categoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteEstadoArticuloAsync(int id)
        {
            try
            {
                var estado = await _unitOfWork.Repository<EstadoArticulo>().GetByIdAsync(id);
                if (estado == null)
                {
                    return new ResponseDto<bool>("Estado de artículo no encontrado");
                }

                estado.Activo = false;
                _unitOfWork.Repository<EstadoArticulo>().Update(estado);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Estado de artículo eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar estado de artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteEstadoPrestamoAsync(int id)
        {
            try
            {
                var estado = await _unitOfWork.Repository<EstadoPrestamo>().GetByIdAsync(id);
                if (estado == null)
                {
                    return new ResponseDto<bool>("Estado de préstamo no encontrado");
                }

                estado.Activo = false;
                _unitOfWork.Repository<EstadoPrestamo>().Update(estado);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Estado de préstamo eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar estado de préstamo: {ex.Message}");
            }
        }
    }
}
