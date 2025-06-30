namespace XWY.WebAPI.Business.DTOs
{
    public class EstadoPrestamoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }

    public class EstadoPrestamoCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class EstadoPrestamoUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
