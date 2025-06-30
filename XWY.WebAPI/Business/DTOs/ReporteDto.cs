namespace XWY.WebAPI.Business.DTOs
{
    public class ReporteArticulosDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Equipo { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public string Ubicacion { get; set; }
        public int Stock { get; set; }
        public decimal? Precio { get; set; }
    }

    public class ReportePrestamosDto
    {
        public string CodigoArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaEntregaEstimada { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string EstadoPrestamo { get; set; }
        public string AprobadoPor { get; set; }
    }

    public class ReporteParametrosDto
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? CategoriaId { get; set; }
        public int? EstadoArticuloId { get; set; }
        public int? EstadoPrestamoId { get; set; }
        public string TipoReporte { get; set; }
        public string Formato { get; set; }
    }
}
