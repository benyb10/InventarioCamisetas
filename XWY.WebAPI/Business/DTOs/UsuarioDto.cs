namespace XWY.WebAPI.Business.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int RolId { get; set; }
        public string RolNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }
        public bool Activo { get; set; }
    }

    public class UsuarioCreateDto
    {
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
    }

    public class UsuarioUpdateDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int RolId { get; set; }
        public bool Activo { get; set; }
    }

    public class UsuarioLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UsuarioPasswordChangeDto
    {
        public int Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
