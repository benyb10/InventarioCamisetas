using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class ArticuloConfiguration : IEntityTypeConfiguration<Articulo>
    {
        public void Configure(EntityTypeBuilder<Articulo> builder)
        {
            builder.ToTable("Articulos");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(a => a.Codigo)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Codigo");

            builder.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("Nombre");

            builder.Property(a => a.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("Descripcion");

            builder.Property(a => a.Equipo)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Equipo");

            builder.Property(a => a.Temporada)
                .HasMaxLength(20)
                .HasColumnName("Temporada");

            builder.Property(a => a.Talla)
                .HasMaxLength(10)
                .HasColumnName("Talla");

            builder.Property(a => a.Color)
                .HasMaxLength(50)
                .HasColumnName("Color");

            builder.Property(a => a.Precio)
                .HasColumnType("decimal(10,2)")
                .HasColumnName("Precio");

            builder.Property(a => a.CategoriaId)
                .IsRequired()
                .HasColumnName("CategoriaId");

            builder.Property(a => a.EstadoArticuloId)
                .IsRequired()
                .HasColumnName("EstadoArticuloId");

            builder.Property(a => a.Ubicacion)
                .HasMaxLength(100)
                .HasColumnName("Ubicacion");

            builder.Property(a => a.Stock)
                .IsRequired()
                .HasDefaultValue(1)
                .HasColumnName("Stock");

            builder.Property(a => a.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaCreacion");

            builder.Property(a => a.FechaActualizacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaActualizacion");

            builder.Property(a => a.Activo)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .HasColumnName("Activo");

            builder.HasIndex(a => a.Codigo)
                .IsUnique()
                .HasDatabaseName("IX_Articulos_Codigo");

            builder.HasIndex(a => a.Equipo)
                .HasDatabaseName("IX_Articulos_Equipo");

            builder.HasOne(a => a.Categoria)
                .WithMany(c => c.Articulos)
                .HasForeignKey(a => a.CategoriaId)
                .HasConstraintName("FK_Articulos_Categorias")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.EstadoArticulo)
                .WithMany(e => e.Articulos)
                .HasForeignKey(a => a.EstadoArticuloId)
                .HasConstraintName("FK_Articulos_EstadosArticulo")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Prestamos)
                .WithOne(p => p.Articulo)
                .HasForeignKey(p => p.ArticuloId)
                .HasConstraintName("FK_Prestamos_Articulos")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(a => a.EstaDisponible);
            builder.Ignore(a => a.NombreCompletoConEquipo);
        }
    }
}
