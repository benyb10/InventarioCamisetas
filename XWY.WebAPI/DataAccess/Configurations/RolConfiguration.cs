using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(r => r.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Nombre");

            builder.Property(r => r.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("Descripcion");

            builder.Property(r => r.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaCreacion");

            builder.Property(r => r.Activo)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .HasColumnName("Activo");

            builder.HasIndex(r => r.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_Roles_Nombre");

            builder.HasMany(r => r.Usuarios)
                .WithOne(u => u.Rol)
                .HasForeignKey(u => u.RolId)
                .HasConstraintName("FK_Usuarios_Roles")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
