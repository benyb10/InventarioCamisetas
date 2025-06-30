using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XWY.WebAPI.Entities
{
    [Table("Prestamos")]
    public class Prestamo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ArticuloId { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Required]
        public DateTime FechaEntregaEstimada { get; set; }

        public DateTime? FechaEntregaReal { get; set; }

        public DateTime? FechaDevolucionEstimada { get; set; }

        public DateTime? FechaDevolucionReal { get; set; }

        [Required]
        public int EstadoPrestamoId { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public int? AprobadoPor { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        [MaxLength(500)]
        public string? ObservacionesAprobacion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("ArticuloId")]
        public virtual Articulo Articulo { get; set; }

        [ForeignKey("EstadoPrestamoId")]
        public virtual EstadoPrestamo EstadoPrestamo { get; set; }

        [ForeignKey("AprobadoPor")]
        public virtual Usuario? UsuarioAprobador { get; set; }

        [NotMapped]
        public bool EstaVencido => FechaDevolucionEstimada.HasValue &&
                                   DateTime.Now > FechaDevolucionEstimada.Value &&
                                   !FechaDevolucionReal.HasValue;

        [NotMapped]
        public int DiasVencido => EstaVencido ?
                                 (int)(DateTime.Now - FechaDevolucionEstimada!.Value).TotalDays : 0;

    }
}
