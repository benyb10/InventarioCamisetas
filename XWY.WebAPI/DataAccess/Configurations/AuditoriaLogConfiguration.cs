using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLog>
    {
        public void Configure(EntityTypeBuilder<AuditoriaLog> builder)
        {
            builder.ToTable("AuditoriaLog");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(a => a.UsuarioId)
                .HasColumnName("UsuarioId");

            builder.Property(a => a.Accion)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Accion");

            builder.Property(a => a.Tabla)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Tabla");

            builder.Property(a => a.RegistroId)
                .HasColumnName("RegistroId");

            builder.Property(a => a.ValoresAnteriores)
                .HasColumnType("NVARCHAR(MAX)")
                .HasColumnName("ValoresAnteriores");

            builder.Property(a => a.ValoresNuevos)
                .HasColumnType("NVARCHAR(MAX)")
                .HasColumnName("ValoresNuevos");

            builder.Property(a => a.DireccionIP)
                .HasMaxLength(45)
                .HasColumnName("DireccionIP");

            builder.Property(a => a.UserAgent)
                .HasMaxLength(500)
                .HasColumnName("UserAgent");

            builder.Property(a => a.FechaAccion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaAccion");

            builder.HasOne(a => a.Usuario)
                .WithMany(u => u.AuditoriasLog)
                .HasForeignKey(a => a.UsuarioId)
                .HasConstraintName("FK_AuditoriaLog_Usuarios")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
