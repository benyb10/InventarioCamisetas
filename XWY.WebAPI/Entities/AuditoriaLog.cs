using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XWY.WebAPI.Entities
{
    [Table("AuditoriaLog")]
    public class AuditoriaLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Accion { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tabla { get; set; }

        public int? RegistroId { get; set; }

        public string? ValoresAnteriores { get; set; }

        public string? ValoresNuevos { get; set; }

        [MaxLength(45)]
        public string? DireccionIP { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public DateTime FechaAccion { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
