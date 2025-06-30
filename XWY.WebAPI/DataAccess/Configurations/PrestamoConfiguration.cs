using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
    {
        public void Configure(EntityTypeBuilder<Prestamo> builder)
        {
            builder.ToTable("Prestamos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.UsuarioId)
                .IsRequired()
                .HasColumnName("UsuarioId");

            builder.Property(p => p.ArticuloId)
                .IsRequired()
                .HasColumnName("ArticuloId");

            builder.Property(p => p.FechaSolicitud)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaSolicitud");

            builder.Property(p => p.FechaEntregaEstimada)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasColumnName("FechaEntregaEstimada");

            builder.Property(p => p.FechaEntregaReal)
                .HasColumnType("datetime2")
                .HasColumnName("FechaEntregaReal");

            builder.Property(p => p.FechaDevolucionEstimada)
                .HasColumnType("datetime2")
                .HasColumnName("FechaDevolucionEstimada");

            builder.Property(p => p.FechaDevolucionReal)
                .HasColumnType("datetime2")
                .HasColumnName("FechaDevolucionReal");

            builder.Property(p => p.EstadoPrestamoId)
                .IsRequired()
                .HasColumnName("EstadoPrestamoId");

            builder.Property(p => p.Observaciones)
                .HasMaxLength(500)
                .HasColumnName("Observaciones");

            builder.Property(p => p.AprobadoPor)
                .HasColumnName("AprobadoPor");

            builder.Property(p => p.FechaAprobacion)
                .HasColumnType("datetime2")
                .HasColumnName("FechaAprobacion");

            builder.Property(p => p.ObservacionesAprobacion)
                .HasMaxLength(500)
                .HasColumnName("ObservacionesAprobacion");

            builder.Property(p => p.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaCreacion");

            builder.Property(p => p.FechaActualizacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaActualizacion");

            builder.HasIndex(p => p.FechaSolicitud)
                .HasDatabaseName("IX_Prestamos_FechaSolicitud");

            builder.HasIndex(p => p.EstadoPrestamoId)
                .HasDatabaseName("IX_Prestamos_EstadoPrestamoId");

            builder.HasOne(p => p.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(p => p.UsuarioId)
                .HasConstraintName("FK_Prestamos_Usuarios")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Articulo)
                .WithMany(a => a.Prestamos)
                .HasForeignKey(p => p.ArticuloId)
                .HasConstraintName("FK_Prestamos_Articulos")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.EstadoPrestamo)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.EstadoPrestamoId)
                .HasConstraintName("FK_Prestamos_EstadosPrestamo")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UsuarioAprobador)
                .WithMany(u => u.PrestamosAprobados)
                .HasForeignKey(p => p.AprobadoPor)
                .HasConstraintName("FK_Prestamos_AprobadoPor")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(p => p.EstaVencido);
            builder.Ignore(p => p.DiasVencido);
        }
    }
}
