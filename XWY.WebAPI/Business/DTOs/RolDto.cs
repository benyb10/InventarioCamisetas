namespace XWY.WebAPI.Business.DTOs
{
    public class RolDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }

    public class RolCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class RolUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
