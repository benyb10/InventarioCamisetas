using AutoMapper;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Business.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrestamoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<PagedResponseDto<PrestamoDto>>> GetAllPagedAsync(PrestamoFiltroDto filtro)
        {
            try
            {
                var totalRegistros = await _unitOfWork.Prestamos.CountAsync();

                var prestamos = await _unitOfWork.Prestamos.GetPagedWithFiltersAsync(
                    filtro.Pagina,
                    filtro.RegistrosPorPagina,
                    filtro.UsuarioId,
                    filtro.ArticuloId,
                    filtro.EstadoPrestamoId,
                    filtro.FechaDesde,
                    filtro.FechaHasta,
                    filtro.OrdenarPor,
                    !filtro.Descendente);

                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                var pagedResponse = new PagedResponseDto<PrestamoDto>(prestamosDto, totalRegistros, filtro.Pagina, filtro.RegistrosPorPagina);

                return new ResponseDto<PagedResponseDto<PrestamoDto>>(pagedResponse);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PagedResponseDto<PrestamoDto>>($"Error al obtener préstamos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> GetByIdAsync(int id)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(id);
                if (prestamo == null)
                {
                    return new ResponseDto<PrestamoDto>("Préstamo no encontrado");
                }

                var prestamoDto = _mapper.Map<PrestamoDto>(prestamo);
                return new ResponseDto<PrestamoDto>(prestamoDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al obtener préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> CreateAsync(PrestamoCreateDto prestamoCreateDto)
        {
            try
            {
                var articulo = await _unitOfWork.Articulos.GetByIdAsync(prestamoCreateDto.ArticuloId);
                if (articulo == null || !articulo.Activo)
                {
                    return new ResponseDto<PrestamoDto>("Artículo no encontrado");
                }

                if (!articulo.EstaDisponible)
                {
                    return new ResponseDto<PrestamoDto>("El artículo no está disponible");
                }

                if (await _unitOfWork.Prestamos.UserHasActiveLoanAsync(prestamoCreateDto.UsuarioId, prestamoCreateDto.ArticuloId))
                {
                    return new ResponseDto<PrestamoDto>("El usuario ya tiene un préstamo activo de este artículo");
                }

                var estadoPendiente = await _unitOfWork.Repository<EstadoPrestamo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Pendiente");

                var prestamo = _mapper.Map<Prestamo>(prestamoCreateDto);
                prestamo.EstadoPrestamoId = estadoPendiente.Id;
                prestamo.FechaSolicitud = DateTime.Now;
                prestamo.FechaCreacion = DateTime.Now;
                prestamo.FechaActualizacion = DateTime.Now;

                await _unitOfWork.Prestamos.AddAsync(prestamo);
                await _unitOfWork.SaveChangesAsync();

                var prestamoConRelaciones = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(prestamo.Id);
                var prestamoDto = _mapper.Map<PrestamoDto>(prestamoConRelaciones);

                return new ResponseDto<PrestamoDto>(prestamoDto, "Préstamo solicitado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al crear préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> UpdateAsync(PrestamoUpdateDto prestamoUpdateDto)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamoUpdateDto.Id);
                if (prestamo == null)
                {
                    return new ResponseDto<PrestamoDto>("Préstamo no encontrado");
                }

                prestamo.FechaEntregaEstimada = prestamoUpdateDto.FechaEntregaEstimada;
                prestamo.FechaDevolucionEstimada = prestamoUpdateDto.FechaDevolucionEstimada;
                prestamo.Observaciones = prestamoUpdateDto.Observaciones;
                prestamo.FechaActualizacion = DateTime.Now;

                _unitOfWork.Prestamos.Update(prestamo);
                await _unitOfWork.SaveChangesAsync();

                var prestamoConRelaciones = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(prestamo.Id);
                var prestamoDto = _mapper.Map<PrestamoDto>(prestamoConRelaciones);

                return new ResponseDto<PrestamoDto>(prestamoDto, "Préstamo actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al actualizar préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
                if (prestamo == null)
                {
                    return new ResponseDto<bool>("Préstamo no encontrado");
                }

                var estadoPendiente = await _unitOfWork.Repository<EstadoPrestamo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Pendiente");

                if (prestamo.EstadoPrestamoId != estadoPendiente.Id)
                {
                    return new ResponseDto<bool>("Solo se pueden eliminar préstamos pendientes");
                }

                _unitOfWork.Prestamos.Remove(prestamo);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Préstamo eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> ApproveAsync(PrestamoAprobacionDto aprobacionDto)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(aprobacionDto.Id);
                if (prestamo == null)
                {
                    return new ResponseDto<PrestamoDto>("Préstamo no encontrado");
                }

                var estadoAprobado = await _unitOfWork.Repository<EstadoPrestamo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Aprobado");

                prestamo.EstadoPrestamoId = estadoAprobado.Id;
                prestamo.AprobadoPor = aprobacionDto.AprobadoPor;
                prestamo.FechaAprobacion = DateTime.Now;
                prestamo.ObservacionesAprobacion = aprobacionDto.ObservacionesAprobacion;
                prestamo.FechaActualizacion = DateTime.Now;

                if (aprobacionDto.FechaEntregaReal.HasValue)
                {
                    var estadoEntregado = await _unitOfWork.Repository<EstadoPrestamo>()
                        .FirstOrDefaultAsync(e => e.Nombre == "Entregado");

                    prestamo.EstadoPrestamoId = estadoEntregado.Id;
                    prestamo.FechaEntregaReal = aprobacionDto.FechaEntregaReal.Value;

                    var articulo = await _unitOfWork.Articulos.GetByIdAsync(prestamo.ArticuloId);
                    var estadoPrestado = await _unitOfWork.Repository<EstadoArticulo>()
                        .FirstOrDefaultAsync(e => e.Nombre == "Prestado");

                    articulo.EstadoArticuloId = estadoPrestado.Id;
                    _unitOfWork.Articulos.Update(articulo);
                }

                _unitOfWork.Prestamos.Update(prestamo);
                await _unitOfWork.SaveChangesAsync();

                var prestamoConRelaciones = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(prestamo.Id);
                var prestamoDto = _mapper.Map<PrestamoDto>(prestamoConRelaciones);

                return new ResponseDto<PrestamoDto>(prestamoDto, "Préstamo aprobado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al aprobar préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> RejectAsync(int id, int rechazadoPor, string observaciones)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
                if (prestamo == null)
                {
                    return new ResponseDto<PrestamoDto>("Préstamo no encontrado");
                }

                var estadoRechazado = await _unitOfWork.Repository<EstadoPrestamo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Rechazado");

                prestamo.EstadoPrestamoId = estadoRechazado.Id;
                prestamo.AprobadoPor = rechazadoPor;
                prestamo.FechaAprobacion = DateTime.Now;
                prestamo.ObservacionesAprobacion = observaciones;
                prestamo.FechaActualizacion = DateTime.Now;

                _unitOfWork.Prestamos.Update(prestamo);
                await _unitOfWork.SaveChangesAsync();

                var prestamoConRelaciones = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(prestamo.Id);
                var prestamoDto = _mapper.Map<PrestamoDto>(prestamoConRelaciones);

                return new ResponseDto<PrestamoDto>(prestamoDto, "Préstamo rechazado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al rechazar préstamo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> DeliverAsync(int id, DateTime fechaEntrega)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
                if (prestamo == null)
                {
                    return new ResponseDto<PrestamoDto>("Préstamo no encontrado");
                }

                var estadoEntregado = await _unitOfWork.Repository<EstadoPrestamo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Entregado");

                prestamo.EstadoPrestamoId = estadoEntregado.Id;
                prestamo.FechaEntregaReal = fechaEntrega;
                prestamo.FechaActualizacion = DateTime.Now;

                var articulo = await _unitOfWork.Articulos.GetByIdAsync(prestamo.ArticuloId);
                var estadoPrestado = await _unitOfWork.Repository<EstadoArticulo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Prestado");

                articulo.EstadoArticuloId = estadoPrestado.Id;
                _unitOfWork.Articulos.Update(articulo);

                _unitOfWork.Prestamos.Update(prestamo);
                await _unitOfWork.SaveChangesAsync();

                var prestamoConRelaciones = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(prestamo.Id);
                var prestamoDto = _mapper.Map<PrestamoDto>(prestamoConRelaciones);

                return new ResponseDto<PrestamoDto>(prestamoDto, "Artículo entregado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al entregar artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PrestamoDto>> ReturnAsync(PrestamoDevolucionDto devolucionDto)
        {
            try
            {
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(devolucionDto.Id);
                if (prestamo == null)
                {
                    return new ResponseDto<PrestamoDto>("Préstamo no encontrado");
                }

                var estadoDevuelto = await _unitOfWork.Repository<EstadoPrestamo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Devuelto");

                prestamo.EstadoPrestamoId = estadoDevuelto.Id;
                prestamo.FechaDevolucionReal = devolucionDto.FechaDevolucionReal;
                prestamo.ObservacionesAprobacion = devolucionDto.ObservacionesDevolucion;
                prestamo.FechaActualizacion = DateTime.Now;

                var articulo = await _unitOfWork.Articulos.GetByIdAsync(prestamo.ArticuloId);
                var estadoDisponible = await _unitOfWork.Repository<EstadoArticulo>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Disponible");

                articulo.EstadoArticuloId = estadoDisponible.Id;
                _unitOfWork.Articulos.Update(articulo);

                _unitOfWork.Prestamos.Update(prestamo);
                await _unitOfWork.SaveChangesAsync();

                var prestamoConRelaciones = await _unitOfWork.Prestamos.GetByIdWithRelationsAsync(prestamo.Id);
                var prestamoDto = _mapper.Map<PrestamoDto>(prestamoConRelaciones);

                return new ResponseDto<PrestamoDto>(prestamoDto, "Artículo devuelto exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<PrestamoDto>($"Error al devolver artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetByUsuarioAsync(usuarioId);
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos del usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetByArticuloAsync(int articuloId)
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetByArticuloAsync(articuloId);
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos del artículo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetPendientesAsync()
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetPendientesAsync();
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos pendientes: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetAprobadosAsync()
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetAprobadosAsync();
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos aprobados: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetEntregadosAsync()
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetEntregadosAsync();
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos entregados: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetVencidosAsync()
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetVencidosAsync();
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos vencidos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetActivosAsync()
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetActivosAsync();
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener préstamos activos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<PrestamoDto>>> GetHistorialAsync()
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetHistorialAsync();
                var prestamosDto = _mapper.Map<List<PrestamoDto>>(prestamos);
                return new ResponseDto<List<PrestamoDto>>(prestamosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<PrestamoDto>>($"Error al obtener historial de préstamos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> HasActiveLoanAsync(int articuloId)
        {
            try
            {
                var hasActiveLoan = await _unitOfWork.Prestamos.HasActiveLoanAsync(articuloId);
                return new ResponseDto<bool>(hasActiveLoan);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al verificar préstamo activo: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> UserHasActiveLoanAsync(int usuarioId, int articuloId)
        {
            try
            {
                var hasActiveLoan = await _unitOfWork.Prestamos.UserHasActiveLoanAsync(usuarioId, articuloId);
                return new ResponseDto<bool>(hasActiveLoan);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al verificar préstamo activo del usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetTotalPrestamosAsync()
        {
            try
            {
                var total = await _unitOfWork.Prestamos.GetTotalPrestamosAsync();
                return new ResponseDto<int>(total);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al obtener total de préstamos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetPendientesCountAsync()
        {
            try
            {
                var count = await _unitOfWork.Prestamos.GetPendientesCountAsync();
                return new ResponseDto<int>(count);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al contar préstamos pendientes: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetVencidosCountAsync()
        {
            try
            {
                var count = await _unitOfWork.Prestamos.GetVencidosCountAsync();
                return new ResponseDto<int>(count);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al contar préstamos vencidos: {ex.Message}");
            }
        }
    }
}
