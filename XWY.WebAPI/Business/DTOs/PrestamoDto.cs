namespace XWY.WebAPI.Business.DTOs
{
    public class PrestamoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }
        public int ArticuloId { get; set; }
        public string ArticuloNombre { get; set; }
        public string ArticuloCodigo { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaEntregaEstimada { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
        public DateTime? FechaDevolucionEstimada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public int EstadoPrestamoId { get; set; }
        public string EstadoPrestamoNombre { get; set; }
        public string Observaciones { get; set; }
        public int? AprobadoPor { get; set; }
        public string AprobadoPorNombre { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string ObservacionesAprobacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class PrestamoCreateDto
    {
        public int UsuarioId { get; set; }
        public int ArticuloId { get; set; }
        public DateTime FechaEntregaEstimada { get; set; }
        public DateTime? FechaDevolucionEstimada { get; set; }
        public string Observaciones { get; set; }
    }

    public class PrestamoUpdateDto
    {
        public int Id { get; set; }
        public DateTime FechaEntregaEstimada { get; set; }
        public DateTime? FechaDevolucionEstimada { get; set; }
        public string Observaciones { get; set; }
    }

    public class PrestamoAprobacionDto
    {
        public int Id { get; set; }
        public bool Aprobado { get; set; }
        public int AprobadoPor { get; set; }
        public string ObservacionesAprobacion { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
    }

    public class PrestamoDevolucionDto
    {
        public int Id { get; set; }
        public DateTime FechaDevolucionReal { get; set; }
        public string ObservacionesDevolucion { get; set; }
    }

    public class PrestamoFiltroDto
    {
        public int? UsuarioId { get; set; }
        public int? ArticuloId { get; set; }
        public int? EstadoPrestamoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 10;
        public string? OrdenarPor { get; set; } = "FechaSolicitud";
        public bool Descendente { get; set; } = true;
    }
}
