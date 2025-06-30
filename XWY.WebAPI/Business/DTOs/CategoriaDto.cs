namespace XWY.WebAPI.Business.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }

    public class CategoriaCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class CategoriaUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
