using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(u => u.Cedula)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("Cedula");

            builder.Property(u => u.Nombres)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Nombres");

            builder.Property(u => u.Apellidos)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Apellidos");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("Email");

            builder.Property(u => u.Telefono)
                .HasMaxLength(15)
                .HasColumnName("Telefono");

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("PasswordHash");

            builder.Property(u => u.RolId)
                .IsRequired()
                .HasColumnName("RolId");

            builder.Property(u => u.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaCreacion");

            builder.Property(u => u.FechaUltimoAcceso)
                .HasColumnType("datetime2")
                .HasColumnName("FechaUltimoAcceso");

            builder.Property(u => u.Activo)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .HasColumnName("Activo");

            builder.HasIndex(u => u.Cedula)
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_Cedula");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_Email");

            builder.HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .HasConstraintName("FK_Usuarios_Roles")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Prestamos)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .HasConstraintName("FK_Prestamos_Usuarios")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.PrestamosAprobados)
                .WithOne(p => p.UsuarioAprobador)
                .HasForeignKey(p => p.AprobadoPor)
                .HasConstraintName("FK_Prestamos_AprobadoPor")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.AuditoriasLog)
                .WithOne(a => a.Usuario)
                .HasForeignKey(a => a.UsuarioId)
                .HasConstraintName("FK_AuditoriaLog_Usuarios")
                .OnDelete(DeleteBehavior.SetNull);

            builder.Ignore(u => u.NombreCompleto);
        }
    }
}
