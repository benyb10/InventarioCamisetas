using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class EstadoPrestamoConfiguration : IEntityTypeConfiguration<EstadoPrestamo>
    {
        public void Configure(EntityTypeBuilder<EstadoPrestamo> builder)
        {
            builder.ToTable("EstadosPrestamo");

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
                .HasDatabaseName("IX_EstadosPrestamo_Nombre");

            builder.HasMany(e => e.Prestamos)
                .WithOne(p => p.EstadoPrestamo)
                .HasForeignKey(p => p.EstadoPrestamoId)
                .HasConstraintName("FK_Prestamos_EstadosPrestamo")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
