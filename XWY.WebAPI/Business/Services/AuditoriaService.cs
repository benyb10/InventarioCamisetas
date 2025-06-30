using AutoMapper;
using Newtonsoft.Json;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Business.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuditoriaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<PagedResponseDto<AuditoriaLogDto>>> GetAllPagedAsync(AuditoriaLogFiltroDto filtro)
        {
            try
            {
                var totalRegistros = await _unitOfWork.Repository<AuditoriaLog>().CountAsync();

                var query = _unitOfWork.Repository<AuditoriaLog>().Query();

                if (filtro.UsuarioId.HasValue)
                    query = query.Where(a => a.UsuarioId == filtro.UsuarioId.Value);

                if (!string.IsNullOrEmpty(filtro.Accion))
                    query = query.Where(a => a.Accion.Contains(filtro.Accion));

                if (!string.IsNullOrEmpty(filtro.Tabla))
                    query = query.Where(a => a.Tabla.Contains(filtro.Tabla));

                if (filtro.FechaDesde.HasValue)
                    query = query.Where(a => a.FechaAccion >= filtro.FechaDesde.Value);

                if (filtro.FechaHasta.HasValue)
                    query = query.Where(a => a.FechaAccion <= filtro.FechaHasta.Value);

                var auditoriasQuery = query.OrderByDescending(a => a.FechaAccion)
                    .Skip((filtro.Pagina - 1) * filtro.RegistrosPorPagina)
                    .Take(filtro.RegistrosPorPagina);

                var auditorias = auditoriasQuery.ToList();
                var auditoriasDto = new List<AuditoriaLogDto>();

                foreach (var auditoria in auditorias)
                {
                    var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                    if (auditoria.UsuarioId.HasValue)
                    {
                        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(auditoria.UsuarioId.Value);
                        auditoriaDto.UsuarioNombre = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "Usuario eliminado";
                    }
                    auditoriasDto.Add(auditoriaDto);
                }

                var pagedResponse = new PagedResponseDto<AuditoriaLogDto>(auditoriasDto, totalRegistros, filtro.Pagina, filtro.RegistrosPorPagina);
                return new ResponseDto<PagedResponseDto<AuditoriaLogDto>>(pagedResponse);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PagedResponseDto<AuditoriaLogDto>>($"Error al obtener auditoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<AuditoriaLogDto>> GetByIdAsync(int id)
        {
            try
            {
                var auditoria = await _unitOfWork.Repository<AuditoriaLog>().GetByIdAsync(id);
                if (auditoria == null)
                {
                    return new ResponseDto<AuditoriaLogDto>("Registro de auditoría no encontrado");
                }

                var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                if (auditoria.UsuarioId.HasValue)
                {
                    var usuario = await _unitOfWork.Usuarios.GetByIdAsync(auditoria.UsuarioId.Value);
                    auditoriaDto.UsuarioNombre = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "Usuario eliminado";
                }

                return new ResponseDto<AuditoriaLogDto>(auditoriaDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<AuditoriaLogDto>($"Error al obtener registro de auditoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<AuditoriaLogDto>> CreateAsync(AuditoriaLogCreateDto auditoriaCreateDto)
        {
            try
            {
                var auditoria = _mapper.Map<AuditoriaLog>(auditoriaCreateDto);
                auditoria.FechaAccion = DateTime.Now;

                await _unitOfWork.Repository<AuditoriaLog>().AddAsync(auditoria);
                await _unitOfWork.SaveChangesAsync();

                var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                return new ResponseDto<AuditoriaLogDto>(auditoriaDto, "Registro de auditoría creado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<AuditoriaLogDto>($"Error al crear registro de auditoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<AuditoriaLogDto>>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                var auditorias = await _unitOfWork.Repository<AuditoriaLog>()
                    .FindAsync(a => a.UsuarioId == usuarioId);

                var auditoriasDto = new List<AuditoriaLogDto>();
                foreach (var auditoria in auditorias.OrderByDescending(a => a.FechaAccion))
                {
                    var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                    var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
                    auditoriaDto.UsuarioNombre = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "Usuario eliminado";
                    auditoriasDto.Add(auditoriaDto);
                }

                return new ResponseDto<List<AuditoriaLogDto>>(auditoriasDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<AuditoriaLogDto>>($"Error al obtener auditoría por usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<AuditoriaLogDto>>> GetByTablaAsync(string tabla)
        {
            try
            {
                var auditorias = await _unitOfWork.Repository<AuditoriaLog>()
                    .FindAsync(a => a.Tabla == tabla);

                var auditoriasDto = new List<AuditoriaLogDto>();
                foreach (var auditoria in auditorias.OrderByDescending(a => a.FechaAccion))
                {
                    var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                    if (auditoria.UsuarioId.HasValue)
                    {
                        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(auditoria.UsuarioId.Value);
                        auditoriaDto.UsuarioNombre = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "Usuario eliminado";
                    }
                    auditoriasDto.Add(auditoriaDto);
                }

                return new ResponseDto<List<AuditoriaLogDto>>(auditoriasDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<AuditoriaLogDto>>($"Error al obtener auditoría por tabla: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<AuditoriaLogDto>>> GetByAccionAsync(string accion)
        {
            try
            {
                var auditorias = await _unitOfWork.Repository<AuditoriaLog>()
                    .FindAsync(a => a.Accion == accion);
                var auditoriasDto = new List<AuditoriaLogDto>();
                foreach (var auditoria in auditorias.OrderByDescending(a => a.FechaAccion))
                {
                    var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                    if (auditoria.UsuarioId.HasValue)
                    {
                        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(auditoria.UsuarioId.Value);
                        auditoriaDto.UsuarioNombre = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "Usuario eliminado";
                    }
                    auditoriasDto.Add(auditoriaDto);
                }

                return new ResponseDto<List<AuditoriaLogDto>>(auditoriasDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<AuditoriaLogDto>>($"Error al obtener auditoría por acción: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<AuditoriaLogDto>>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var auditorias = await _unitOfWork.Repository<AuditoriaLog>()
                    .FindAsync(a => a.FechaAccion >= fechaInicio && a.FechaAccion <= fechaFin);

                var auditoriasDto = new List<AuditoriaLogDto>();
                foreach (var auditoria in auditorias.OrderByDescending(a => a.FechaAccion))
                {
                    var auditoriaDto = _mapper.Map<AuditoriaLogDto>(auditoria);
                    if (auditoria.UsuarioId.HasValue)
                    {
                        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(auditoria.UsuarioId.Value);
                        auditoriaDto.UsuarioNombre = usuario != null ? $"{usuario.Nombres} {usuario.Apellidos}" : "Usuario eliminado";
                    }
                    auditoriasDto.Add(auditoriaDto);
                }

                return new ResponseDto<List<AuditoriaLogDto>>(auditoriasDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<AuditoriaLogDto>>($"Error al obtener auditoría por rango de fechas: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> LogAccionAsync(int? usuarioId, string accion, string tabla, int? registroId, object valoresAnteriores, object valoresNuevos, string direccionIP, string userAgent)
        {
            try
            {
                var auditoria = new AuditoriaLog
                {
                    UsuarioId = usuarioId,
                    Accion = accion,
                    Tabla = tabla,
                    RegistroId = registroId,
                    ValoresAnteriores = valoresAnteriores != null ? JsonConvert.SerializeObject(valoresAnteriores) : null,
                    ValoresNuevos = valoresNuevos != null ? JsonConvert.SerializeObject(valoresNuevos) : null,
                    DireccionIP = direccionIP,
                    UserAgent = userAgent,
                    FechaAccion = DateTime.Now
                };

                await _unitOfWork.Repository<AuditoriaLog>().AddAsync(auditoria);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Acción auditada exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al registrar auditoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetTotalRegistrosAsync()
        {
            try
            {
                var total = await _unitOfWork.Repository<AuditoriaLog>().CountAsync();
                return new ResponseDto<int>(total);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al obtener total de registros de auditoría: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> LimpiarAuditoriaAntiguaAsync(int diasAntiguedad = 365)
        {
            try
            {
                var fechaLimite = DateTime.Now.AddDays(-diasAntiguedad);
                var auditoriasAntiguas = await _unitOfWork.Repository<AuditoriaLog>()
                    .FindAsync(a => a.FechaAccion < fechaLimite);

                if (auditoriasAntiguas.Any())
                {
                    _unitOfWork.Repository<AuditoriaLog>().RemoveRange(auditoriasAntiguas);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseDto<bool>(true, $"Registros de auditoría anteriores a {diasAntiguedad} días eliminados exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al limpiar auditoría antigua: {ex.Message}");
            }
        }
    }
}
