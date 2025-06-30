namespace XWY.WebAPI.Business.DTOs
{
    public class ArticuloDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Equipo { get; set; }
        public string Temporada { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public decimal? Precio { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public int EstadoArticuloId { get; set; }
        public string EstadoArticuloNombre { get; set; }
        public string Ubicacion { get; set; }
        public int Stock { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public bool Activo { get; set; }
    }

    public class ArticuloCreateDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Equipo { get; set; }
        public string Temporada { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public decimal? Precio { get; set; }
        public int CategoriaId { get; set; }
        public int EstadoArticuloId { get; set; }
        public string Ubicacion { get; set; }
        public int Stock { get; set; }
    }

    public class ArticuloUpdateDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Equipo { get; set; }
        public string Temporada { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public decimal? Precio { get; set; }
        public int CategoriaId { get; set; }
        public int EstadoArticuloId { get; set; }
        public string Ubicacion { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
    }

    public class ArticuloFiltroDto
    {
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Equipo { get; set; }
        public int? CategoriaId { get; set; }
        public int? EstadoArticuloId { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 10;
        public string? OrdenarPor { get; set; } = "Nombre";
        public bool Descendente { get; set; } = false;
    }
}
