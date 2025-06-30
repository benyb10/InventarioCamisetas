using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XWY.WebAPI.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Cedula { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombres { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(15)]
        public string? Telefono { get; set; }

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; }

        [Required]
        public int RolId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimoAcceso { get; set; }

        public bool Activo { get; set; } = true;

        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

        public virtual ICollection<Prestamo> PrestamosAprobados { get; set; } = new List<Prestamo>();

        public virtual ICollection<AuditoriaLog> AuditoriasLog { get; set; } = new List<AuditoriaLog>();

        [NotMapped]
        public string NombreCompleto => $"{Nombres} {Apellidos}";

    }
}
