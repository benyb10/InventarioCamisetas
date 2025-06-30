using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XWY.WebAPI.Entities
{
    [Table("Articulos")]
    public class Articulo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Codigo { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Equipo { get; set; }

        [MaxLength(20)]
        public string? Temporada { get; set; }

        [MaxLength(10)]
        public string? Talla { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Precio { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int EstadoArticuloId { get; set; }

        [MaxLength(100)]
        public string? Ubicacion { get; set; }

        public int Stock { get; set; } = 1;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }

        [ForeignKey("EstadoArticuloId")]
        public virtual EstadoArticulo EstadoArticulo { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

        [NotMapped]
        public bool EstaDisponible => EstadoArticulo?.Nombre == "Disponible" && Stock > 0;

        [NotMapped]
        public string NombreCompletoConEquipo => $"{Nombre} - {Equipo}";

    }
}
