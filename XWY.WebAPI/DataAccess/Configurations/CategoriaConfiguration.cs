using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Configurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .UseIdentityColumn(1, 1);

            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Nombre");

            builder.Property(c => c.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("Descripcion");

            builder.Property(c => c.FechaCreacion)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("FechaCreacion");

            builder.Property(c => c.Activo)
                .IsRequired()
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .HasColumnName("Activo");

            builder.HasIndex(c => c.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_Categorias_Nombre");

            builder.HasMany(c => c.Articulos)
                .WithOne(a => a.Categoria)
                .HasForeignKey(a => a.CategoriaId)
                .HasConstraintName("FK_Articulos_Categorias")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
