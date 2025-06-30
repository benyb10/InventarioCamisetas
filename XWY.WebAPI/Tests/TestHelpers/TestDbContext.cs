using Microsoft.EntityFrameworkCore;
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Tests.TestHelpers
{
    public static class TestDbContext
    {
        public static XWYDbContext CreateInMemoryContext(string databaseName = null)
        {
            var dbName = databaseName ?? Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<XWYDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new XWYDbContext(options);

            SeedTestData(context);

            return context;
        }

        public static void SeedTestData(XWYDbContext context)
        {
            if (context.Roles.Any())
                return;

            var roles = new List<Rol>
            {
                new Rol
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Descripcion = "Acceso completo al sistema",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Rol
                {
                    Id = 2,
                    Nombre = "Operador",
                    Descripcion = "Acceso limitado para operaciones básicas",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                }
            };

            var categorias = new List<Categoria>
            {
                new Categoria
                {
                    Id = 1,
                    Nombre = "Internacional",
                    Descripcion = "Equipos internacionales",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Categoria
                {
                    Id = 2,
                    Nombre = "Nacional",
                    Descripcion = "Equipos nacionales",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Categoria
                {
                    Id = 3,
                    Nombre = "Selecciones",
                    Descripcion = "Selecciones nacionales",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Categoria
                {
                    Id = 4,
                    Nombre = "Retro",
                    Descripcion = "Camisetas de temporadas pasadas",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                }
            };

            var estadosArticulo = new List<EstadoArticulo>
            {
                new EstadoArticulo
                {
                    Id = 1,
                    Nombre = "Disponible",
                    Descripcion = "Artículo disponible para préstamo",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoArticulo
                {
                    Id = 2,
                    Nombre = "Prestado",
                    Descripcion = "Artículo actualmente prestado",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoArticulo
                {
                    Id = 3,
                    Nombre = "Mantenimiento",
                    Descripcion = "Artículo en proceso de mantenimiento",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoArticulo
                {
                    Id = 4,
                    Nombre = "Dañado",
                    Descripcion = "Artículo con daños que impiden su uso",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                }
            };

            var estadosPrestamo = new List<EstadoPrestamo>
            {
                new EstadoPrestamo
                {
                    Id = 1,
                    Nombre = "Pendiente",
                    Descripcion = "Préstamo solicitado, pendiente de aprobación",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoPrestamo
                {
                    Id = 2,
                    Nombre = "Aprobado",
                    Descripcion = "Préstamo aprobado, pendiente de entrega",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoPrestamo
                {
                    Id = 3,
                    Nombre = "Entregado",
                    Descripcion = "Artículo entregado al usuario",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoPrestamo
                {
                    Id = 4,
                    Nombre = "Devuelto",
                    Descripcion = "Artículo devuelto correctamente",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoPrestamo
                {
                    Id = 5,
                    Nombre = "Rechazado",
                    Descripcion = "Préstamo rechazado",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new EstadoPrestamo
                {
                    Id = 6,
                    Nombre = "Vencido",
                    Descripcion = "Préstamo con fecha de devolución vencida",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                }
            };

            context.Roles.AddRange(roles);
            context.Categorias.AddRange(categorias);
            context.EstadosArticulo.AddRange(estadosArticulo);
            context.EstadosPrestamo.AddRange(estadosPrestamo);

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Cedula = "1804567891",
                    Nombres = "Carlos Eduardo",
                    Apellidos = "Mendoza Silva",
                    Email = "carlos.mendoza@test.com",
                    Telefono = "0998765432",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RolId = 1,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaUltimoAcceso = DateTime.Now
                },
                new Usuario
                {
                    Id = 2,
                    Cedula = "1705432167",
                    Nombres = "María Fernanda",
                    Apellidos = "Rodríguez Torres",
                    Email = "maria.rodriguez@test.com",
                    Telefono = "0987654321",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                    RolId = 2,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaUltimoAcceso = DateTime.Now
                },
                new Usuario
                {
                    Id = 3,
                    Cedula = "1806789123",
                    Nombres = "Luis Antonio",
                    Apellidos = "García Morales",
                    Email = "luis.garcia@test.com",
                    Telefono = "0976543210",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    RolId = 2,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaUltimoAcceso = DateTime.Now
                }
            };

            var articulos = new List<Articulo>
            {
                new Articulo
                {
                    Id = 1,
                    Codigo = "PSG-001-L",
                    Nombre = "Camiseta PSG Local 2024",
                    Descripcion = "Camiseta oficial del Paris Saint-Germain temporada 2024",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 3,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                },
                new Articulo
                {
                    Id = 2,
                    Codigo = "RMA-002-M",
                    Nombre = "Camiseta Real Madrid Local 2024",
                    Descripcion = "Camiseta oficial del Real Madrid temporada 2024",
                    Equipo = "Real Madrid",
                    Temporada = "2023-2024",
                    Talla = "M",
                    Color = "Blanco",
                    Precio = 94.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-2",
                    Stock = 2,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                },
                new Articulo
                {
                    Id = 3,
                    Codigo = "BAR-003-XL",
                    Nombre = "Camiseta FC Barcelona Local 2024",
                    Descripcion = "Camiseta oficial del FC Barcelona temporada 2024",
                    Equipo = "FC Barcelona",
                    Temporada = "2023-2024",
                    Talla = "XL",
                    Color = "Azul Grana",
                    Precio = 92.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-3",
                    Stock = 4,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                },
                new Articulo
                {
                    Id = 4,
                    Codigo = "ECU-005-M",
                    Nombre = "Camiseta Selección Ecuador 2024",
                    Descripcion = "Camiseta oficial de la Selección de Ecuador",
                    Equipo = "Selección Ecuador",
                    Temporada = "2024",
                    Talla = "M",
                    Color = "Amarillo",
                    Precio = 79.99m,
                    CategoriaId = 3,
                    EstadoArticuloId = 2,
                    Ubicacion = "Estante C-1",
                    Stock = 1,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                },
                new Articulo
                {
                    Id = 5,
                    Codigo = "LIV-007-M",
                    Nombre = "Camiseta Liverpool Local 2024",
                    Descripcion = "Camiseta oficial del Liverpool FC temporada 2024",
                    Equipo = "Liverpool FC",
                    Temporada = "2023-2024",
                    Talla = "M",
                    Color = "Rojo",
                    Precio = 89.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 3,
                    Ubicacion = "Estante B-2",
                    Stock = 1,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                }
            };

            context.Usuarios.AddRange(usuarios);
            context.Articulos.AddRange(articulos);

            var prestamos = new List<Prestamo>
            {
                new Prestamo
                {
                    Id = 1,
                    UsuarioId = 2,
                    ArticuloId = 4,
                    FechaSolicitud = DateTime.Now.AddDays(-10),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-3),
                    FechaEntregaReal = DateTime.Now.AddDays(-2),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(5),
                    EstadoPrestamoId = 3,
                    Observaciones = "Préstamo para evento deportivo",
                    AprobadoPor = 1,
                    FechaAprobacion = DateTime.Now.AddDays(-5),
                    ObservacionesAprobacion = "Aprobado para evento",
                    FechaCreacion = DateTime.Now.AddDays(-10),
                    FechaActualizacion = DateTime.Now.AddDays(-2)
                },
                new Prestamo
                {
                    Id = 2,
                    UsuarioId = 3,
                    ArticuloId = 1,
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaEntregaEstimada = DateTime.Now.AddDays(2),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(9),
                    EstadoPrestamoId = 1,
                    Observaciones = "Préstamo pendiente para partido",
                    FechaCreacion = DateTime.Now.AddDays(-5),
                    FechaActualizacion = DateTime.Now.AddDays(-5)
                },
                new Prestamo
                {
                    Id = 3,
                    UsuarioId = 2,
                    ArticuloId = 3,
                    FechaSolicitud = DateTime.Now.AddDays(-15),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-8),
                    FechaEntregaReal = DateTime.Now.AddDays(-7),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(-1),
                    FechaDevolucionReal = DateTime.Now.AddDays(-1),
                    EstadoPrestamoId = 4,
                    Observaciones = "Préstamo completado",
                    AprobadoPor = 1,
                    FechaAprobacion = DateTime.Now.AddDays(-12),
                    ObservacionesAprobacion = "Aprobado",
                    FechaCreacion = DateTime.Now.AddDays(-15),
                    FechaActualizacion = DateTime.Now.AddDays(-1)
                }
            };

            context.Prestamos.AddRange(prestamos);
            context.SaveChanges();
        }

        public static void CleanDatabase(XWYDbContext context)
        {
            context.AuditoriasLog.RemoveRange(context.AuditoriasLog);
            context.Prestamos.RemoveRange(context.Prestamos);
            context.Articulos.RemoveRange(context.Articulos);
            context.Usuarios.RemoveRange(context.Usuarios);
            context.EstadosPrestamo.RemoveRange(context.EstadosPrestamo);
            context.EstadosArticulo.RemoveRange(context.EstadosArticulo);
            context.Categorias.RemoveRange(context.Categorias);
            context.Roles.RemoveRange(context.Roles);
            context.SaveChanges();
        }
    }
}