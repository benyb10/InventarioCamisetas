namespace XWY.WebAPI.Business.DTOs
{
    public class AuditoriaLogDto
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string Accion { get; set; }
        public string Tabla { get; set; }
        public int? RegistroId { get; set; }
        public string ValoresAnteriores { get; set; }
        public string ValoresNuevos { get; set; }
        public string DireccionIP { get; set; }
        public string UserAgent { get; set; }
        public DateTime FechaAccion { get; set; }
    }

    public class AuditoriaLogCreateDto
    {
        public int? UsuarioId { get; set; }
        public string Accion { get; set; }
        public string Tabla { get; set; }
        public int? RegistroId { get; set; }
        public string ValoresAnteriores { get; set; }
        public string ValoresNuevos { get; set; }
        public string DireccionIP { get; set; }
        public string UserAgent { get; set; }
    }

    public class AuditoriaLogFiltroDto
    {
        public int? UsuarioId { get; set; }
        public string Accion { get; set; }
        public string Tabla { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 10;
    }
}
