using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class EstadoArticuloConfiguration : IEntityTypeConfiguration<EstadoArticulo>
    {
        public void Configure(EntityTypeBuilder<EstadoArticulo> builder)
        {
            builder.ToTable("EstadosArticulo");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Nombre");

            builder.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("Descripcion");

            builder.Property(e => e.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaCreacion");

            builder.Property(e => e.Activo)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .HasColumnName("Activo");

            builder.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_EstadosArticulo_Nombre");

            builder.HasMany(e => e.Articulos)
                .WithOne(a => a.EstadoArticulo)
                .HasForeignKey(a => a.EstadoArticuloId)
                .HasConstraintName("FK_Articulos_EstadosArticulo")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
