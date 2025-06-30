using Microsoft.EntityFrameworkCore;
using XWY.WebAPI.DataAccess.Configurations;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.DataAccess.Context
{
    public class XWYDbContext : DbContext
    {
        public XWYDbContext(DbContextOptions<XWYDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<EstadoArticulo> EstadosArticulo { get; set; }
        public DbSet<EstadoPrestamo> EstadosPrestamo { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<AuditoriaLog> AuditoriasLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RolConfiguration());
            modelBuilder.ApplyConfiguration(new CategoriaConfiguration());
            modelBuilder.ApplyConfiguration(new EstadoArticuloConfiguration());
            modelBuilder.ApplyConfiguration(new EstadoPrestamoConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new ArticuloConfiguration());
            modelBuilder.ApplyConfiguration(new PrestamoConfiguration());
            modelBuilder.ApplyConfiguration(new AuditoriaLogConfiguration());

            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador", Descripcion = "Acceso completo al sistema", FechaCreacion = DateTime.Now, Activo = true },
                new Rol { Id = 2, Nombre = "Operador", Descripcion = "Acceso limitado para operaciones básicas", FechaCreacion = DateTime.Now, Activo = true }
            );

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Camisetas Locales", Descripcion = "Camisetas de equipos locales", FechaCreacion = DateTime.Now, Activo = true },
                new Categoria { Id = 2, Nombre = "Camisetas Internacionales", Descripcion = "Camisetas de equipos internacionales", FechaCreacion = DateTime.Now, Activo = true },
                new Categoria { Id = 3, Nombre = "Camisetas Selecciones", Descripcion = "Camisetas de selecciones nacionales", FechaCreacion = DateTime.Now, Activo = true },
                new Categoria { Id = 4, Nombre = "Camisetas Retro", Descripcion = "Camisetas de temporadas pasadas", FechaCreacion = DateTime.Now, Activo = true }
            );

            modelBuilder.Entity<EstadoArticulo>().HasData(
                new EstadoArticulo { Id = 1, Nombre = "Disponible", Descripcion = "Artículo disponible para préstamo", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoArticulo { Id = 2, Nombre = "Prestado", Descripcion = "Artículo actualmente prestado", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoArticulo { Id = 3, Nombre = "En Mantenimiento", Descripcion = "Artículo en proceso de mantenimiento", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoArticulo { Id = 4, Nombre = "Dañado", Descripcion = "Artículo con daños que impiden su uso", FechaCreacion = DateTime.Now, Activo = true }
            );

            modelBuilder.Entity<EstadoPrestamo>().HasData(
                new EstadoPrestamo { Id = 1, Nombre = "Pendiente", Descripcion = "Préstamo solicitado, pendiente de aprobación", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoPrestamo { Id = 2, Nombre = "Aprobado", Descripcion = "Préstamo aprobado, pendiente de entrega", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoPrestamo { Id = 3, Nombre = "Entregado", Descripcion = "Artículo entregado al usuario", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoPrestamo { Id = 4, Nombre = "Devuelto", Descripcion = "Artículo devuelto correctamente", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoPrestamo { Id = 5, Nombre = "Rechazado", Descripcion = "Préstamo rechazado", FechaCreacion = DateTime.Now, Activo = true },
                new EstadoPrestamo { Id = 6, Nombre = "Vencido", Descripcion = "Préstamo con fecha de devolución vencida", FechaCreacion = DateTime.Now, Activo = true }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Articulo && e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is Articulo articulo)
                {
                    articulo.FechaActualizacion = DateTime.Now;
                }
            }

            var prestamoEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is Prestamo && e.State == EntityState.Modified);

            foreach (var entry in prestamoEntries)
            {
                if (entry.Entity is Prestamo prestamo)
                {
                    prestamo.FechaActualizacion = DateTime.Now;
                }
            }
        }
    }
}
