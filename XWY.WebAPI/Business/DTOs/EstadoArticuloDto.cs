namespace XWY.WebAPI.Business.DTOs
{
    public class EstadoArticuloDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }

    public class EstadoArticuloCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class EstadoArticuloUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
