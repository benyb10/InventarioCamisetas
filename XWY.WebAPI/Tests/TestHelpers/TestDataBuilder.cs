using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Tests.TestHelpers
{
    public static class TestDataBuilder
    {
        public static class Usuarios
        {
            public static Usuario CreateAdmin(int id = 1)
            {
                return new Usuario
                {
                    Id = id,
                    Cedula = "1804567891",
                    Nombres = "Carlos Eduardo",
                    Apellidos = "Mendoza Silva",
                    Email = $"admin{id}@test.com",
                    Telefono = "0998765432",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RolId = 1,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaUltimoAcceso = DateTime.Now
                };
            }

            public static Usuario CreateOperator(int id = 2)
            {
                return new Usuario
                {
                    Id = id,
                    Cedula = "1705432167",
                    Nombres = "María Fernanda",
                    Apellidos = "Rodríguez Torres",
                    Email = $"operator{id}@test.com",
                    Telefono = "0987654321",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                    RolId = 2,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaUltimoAcceso = DateTime.Now
                };
            }

            public static UsuarioCreateDto CreateDto(string email = "nuevo@test.com", string cedula = "1111111111")
            {
                return new UsuarioCreateDto
                {
                    Cedula = cedula,
                    Nombres = "Nuevo",
                    Apellidos = "Usuario",
                    Email = email,
                    Telefono = "0999888777",
                    Password = "password123",
                    RolId = 2
                };
            }

            public static UsuarioUpdateDto UpdateDto(int id = 1)
            {
                return new UsuarioUpdateDto
                {
                    Id = id,
                    Cedula = "1804567891",
                    Nombres = "Carlos Eduardo Updated",
                    Apellidos = "Mendoza Silva",
                    Email = $"updated{id}@test.com",
                    Telefono = "0998765432",
                    RolId = 1,
                    Activo = true
                };
            }

            public static UsuarioLoginDto LoginDto(string email = "admin@test.com", string password = "admin123")
            {
                return new UsuarioLoginDto
                {
                    Email = email,
                    Password = password
                };
            }

            public static UsuarioPasswordChangeDto PasswordChangeDto(int id = 1)
            {
                return new UsuarioPasswordChangeDto
                {
                    Id = id,
                    CurrentPassword = "admin123",
                    NewPassword = "newpassword123"
                };
            }
        }

        public static class Articulos
        {
            public static Articulo CreatePSG(int id = 1)
            {
                return new Articulo
                {
                    Id = id,
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
                };
            }

            public static Articulo CreateRealMadrid(int id = 2)
            {
                return new Articulo
                {
                    Id = id,
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
                };
            }

            public static Articulo CreateBarcelona(int id = 3)
            {
                return new Articulo
                {
                    Id = id,
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
                };
            }

            public static Articulo CreateEcuador(int id = 4)
            {
                return new Articulo
                {
                    Id = id,
                    Codigo = "ECU-005-M",
                    Nombre = "Camiseta Selección Ecuador 2024",
                    Descripcion = "Camiseta oficial de la Selección de Ecuador",
                    Equipo = "Selección Ecuador",
                    Temporada = "2024",
                    Talla = "M",
                    Color = "Amarillo",
                    Precio = 79.99m,
                    CategoriaId = 3,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante C-1",
                    Stock = 5,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                };
            }

            public static ArticuloCreateDto CreateDto(string codigo = "TEST-001-M")
            {
                return new ArticuloCreateDto
                {
                    Codigo = codigo,
                    Nombre = "Camiseta Test 2024",
                    Descripcion = "Camiseta de prueba para testing",
                    Equipo = "Test Team",
                    Temporada = "2023-2024",
                    Talla = "M",
                    Color = "Verde",
                    Precio = 75.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante T-1",
                    Stock = 2
                };
            }

            public static ArticuloUpdateDto UpdateDto(int id = 1, string codigo = "PSG-001-L-UPD")
            {
                return new ArticuloUpdateDto
                {
                    Id = id,
                    Codigo = codigo,
                    Nombre = "Camiseta PSG Local 2024 Updated",
                    Descripcion = "Camiseta oficial actualizada",
                    Equipo = "Paris Saint-Germain",
                    Temporada = "2023-2024",
                    Talla = "L",
                    Color = "Azul Marino",
                    Precio = 99.99m,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    Ubicacion = "Estante A-1",
                    Stock = 5,
                    Activo = true
                };
            }

            public static ArticuloFiltroDto FiltroDto()
            {
                return new ArticuloFiltroDto
                {
                    Pagina = 1,
                    RegistrosPorPagina = 10,
                    OrdenarPor = "Nombre",
                    Descendente = false
                };
            }
        }

        public static class Prestamos
        {
            public static Prestamo CreatePendiente(int id = 1, int usuarioId = 2, int articuloId = 1)
            {
                return new Prestamo
                {
                    Id = id,
                    UsuarioId = usuarioId,
                    ArticuloId = articuloId,
                    FechaSolicitud = DateTime.Now.AddDays(-2),
                    FechaEntregaEstimada = DateTime.Now.AddDays(5),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(12),
                    EstadoPrestamoId = 1,
                    Observaciones = "Préstamo pendiente de aprobación",
                    FechaCreacion = DateTime.Now.AddDays(-2),
                    FechaActualizacion = DateTime.Now.AddDays(-2)
                };
            }

            public static Prestamo CreateAprobado(int id = 2, int usuarioId = 2, int articuloId = 1)
            {
                return new Prestamo
                {
                    Id = id,
                    UsuarioId = usuarioId,
                    ArticuloId = articuloId,
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaEntregaEstimada = DateTime.Now.AddDays(2),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(9),
                    EstadoPrestamoId = 2,
                    Observaciones = "Préstamo aprobado",
                    AprobadoPor = 1,
                    FechaAprobacion = DateTime.Now.AddDays(-3),
                    ObservacionesAprobacion = "Aprobado para evento",
                    FechaCreacion = DateTime.Now.AddDays(-5),
                    FechaActualizacion = DateTime.Now.AddDays(-3)
                };
            }

            public static Prestamo CreateEntregado(int id = 3, int usuarioId = 2, int articuloId = 1)
            {
                return new Prestamo
                {
                    Id = id,
                    UsuarioId = usuarioId,
                    ArticuloId = articuloId,
                    FechaSolicitud = DateTime.Now.AddDays(-10),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-3),
                    FechaEntregaReal = DateTime.Now.AddDays(-2),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(5),
                    EstadoPrestamoId = 3,
                    Observaciones = "Préstamo entregado",
                    AprobadoPor = 1,
                    FechaAprobacion = DateTime.Now.AddDays(-7),
                    ObservacionesAprobacion = "Aprobado",
                    FechaCreacion = DateTime.Now.AddDays(-10),
                    FechaActualizacion = DateTime.Now.AddDays(-2)
                };
            }

            public static Prestamo CreateDevuelto(int id = 4, int usuarioId = 2, int articuloId = 1)
            {
                return new Prestamo
                {
                    Id = id,
                    UsuarioId = usuarioId,
                    ArticuloId = articuloId,
                    FechaSolicitud = DateTime.Now.AddDays(-20),
                    FechaEntregaEstimada = DateTime.Now.AddDays(-13),
                    FechaEntregaReal = DateTime.Now.AddDays(-12),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(-5),
                    FechaDevolucionReal = DateTime.Now.AddDays(-3),
                    EstadoPrestamoId = 4,
                    Observaciones = "Préstamo completado exitosamente",
                    AprobadoPor = 1,
                    FechaAprobacion = DateTime.Now.AddDays(-17),
                    ObservacionesAprobacion = "Aprobado",
                    FechaCreacion = DateTime.Now.AddDays(-20),
                    FechaActualizacion = DateTime.Now.AddDays(-3)
                };
            }

            public static PrestamoCreateDto CreateDto(int usuarioId = 2, int articuloId = 1)
            {
                return new PrestamoCreateDto
                {
                    UsuarioId = usuarioId,
                    ArticuloId = articuloId,
                    FechaEntregaEstimada = DateTime.Now.AddDays(7),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(14),
                    Observaciones = "Préstamo para evento deportivo"
                };
            }

            public static PrestamoUpdateDto UpdateDto(int id = 1)
            {
                return new PrestamoUpdateDto
                {
                    Id = id,
                    FechaEntregaEstimada = DateTime.Now.AddDays(10),
                    FechaDevolucionEstimada = DateTime.Now.AddDays(17),
                    Observaciones = "Observaciones actualizadas para el préstamo"
                };
            }

            public static PrestamoAprobacionDto AprobacionDto(int id = 1, bool aprobado = true)
            {
                return new PrestamoAprobacionDto
                {
                    Id = id,
                    Aprobado = aprobado,
                    AprobadoPor = 1,
                    ObservacionesAprobacion = aprobado ? "Aprobado para el evento" : "No disponible en este momento"
                };
            }

            public static PrestamoDevolucionDto DevolucionDto(int id = 1)
            {
                return new PrestamoDevolucionDto
                {
                    Id = id,
                    FechaDevolucionReal = DateTime.Now,
                    ObservacionesDevolucion = "Devuelto en perfectas condiciones"
                };
            }
        }

        public static class Catalogos
        {
            public static Rol CreateRolAdmin()
            {
                return new Rol
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Descripcion = "Acceso completo al sistema",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static Rol CreateRolOperator()
            {
                return new Rol
                {
                    Id = 2,
                    Nombre = "Operador",
                    Descripcion = "Acceso limitado para operaciones básicas",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static Categoria CreateCategoriaInternacional()
            {
                return new Categoria
                {
                    Id = 1,
                    Nombre = "Internacional",
                    Descripcion = "Equipos internacionales",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static Categoria CreateCategoriaNacional()
            {
                return new Categoria
                {
                    Id = 2,
                    Nombre = "Nacional",
                    Descripcion = "Equipos nacionales",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static EstadoArticulo CreateEstadoDisponible()
            {
                return new EstadoArticulo
                {
                    Id = 1,
                    Nombre = "Disponible",
                    Descripcion = "Artículo disponible para préstamo",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static EstadoArticulo CreateEstadoPrestado()
            {
                return new EstadoArticulo
                {
                    Id = 2,
                    Nombre = "Prestado",
                    Descripcion = "Artículo actualmente prestado",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static EstadoPrestamo CreateEstadoPendiente()
            {
                return new EstadoPrestamo
                {
                    Id = 1,
                    Nombre = "Pendiente",
                    Descripcion = "Préstamo solicitado, pendiente de aprobación",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }

            public static EstadoPrestamo CreateEstadoAprobado()
            {
                return new EstadoPrestamo
                {
                    Id = 2,
                    Nombre = "Aprobado",
                    Descripcion = "Préstamo aprobado, pendiente de entrega",
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
            }
        }

        public static class Reportes
        {
            public static ReporteParametrosDto ParametrosArticulos()
            {
                return new ReporteParametrosDto
                {
                    TipoReporte = "articulos",
                    Formato = "pdf",
                    CategoriaId = null,
                    EstadoArticuloId = null
                };
            }

            public static ReporteParametrosDto ParametrosPrestamos()
            {
                return new ReporteParametrosDto
                {
                    TipoReporte = "prestamos",
                    Formato = "excel",
                    FechaDesde = DateTime.Now.AddDays(-30),
                    FechaHasta = DateTime.Now,
                    EstadoPrestamoId = null
                };
            }

            public static ReporteParametrosDto ParametrosPersonalizados(string tipo = "articulos", string formato = "pdf")
            {
                return new ReporteParametrosDto
                {
                    TipoReporte = tipo,
                    Formato = formato,
                    FechaDesde = DateTime.Now.AddDays(-60),
                    FechaHasta = DateTime.Now,
                    CategoriaId = 1,
                    EstadoArticuloId = 1,
                    EstadoPrestamoId = 1
                };
            }
        }

        public static class AuditoriaLogs
        {
            public static AuditoriaLog CreateLog(int id = 1, int usuarioId = 1)
            {
                return new AuditoriaLog
                {
                    Id = id,
                    UsuarioId = usuarioId,
                    Accion = "CREATE",
                    Tabla = "Articulos",
                    RegistroId = 1,
                    ValoresAnteriores = null,
                    ValoresNuevos = "{\"Codigo\":\"TEST-001\",\"Nombre\":\"Camiseta Test\"}",
                    DireccionIP = "192.168.1.100",
                    UserAgent = "Mozilla/5.0 Test Browser",
                    FechaAccion = DateTime.Now
                };
            }

            public static AuditoriaLog CreateUpdateLog(int id = 2, int usuarioId = 1)
            {
                return new AuditoriaLog
                {
                    Id = id,
                    UsuarioId = usuarioId,
                    Accion = "UPDATE",
                    Tabla = "Prestamos",
                    RegistroId = 1,
                    ValoresAnteriores = "{\"EstadoPrestamoId\":1}",
                    ValoresNuevos = "{\"EstadoPrestamoId\":2}",
                    DireccionIP = "192.168.1.100",
                    UserAgent = "Mozilla/5.0 Test Browser",
                    FechaAccion = DateTime.Now
                };
            }
        }

        public static class MockData
        {
            public static List<Usuario> GetUsuarios()
            {
                return new List<Usuario>
                {
                    Usuarios.CreateAdmin(1),
                    Usuarios.CreateOperator(2),
                    Usuarios.CreateOperator(3)
                };
            }

            public static List<Articulo> GetArticulos()
            {
                return new List<Articulo>
                {
                    Articulos.CreatePSG(1),
                    Articulos.CreateRealMadrid(2),
                    Articulos.CreateBarcelona(3),
                    Articulos.CreateEcuador(4)
                };
            }

            public static List<Prestamo> GetPrestamos()
            {
                return new List<Prestamo>
                {
                    Prestamos.CreatePendiente(1, 2, 1),
                    Prestamos.CreateAprobado(2, 2, 2),
                    Prestamos.CreateEntregado(3, 3, 3),
                    Prestamos.CreateDevuelto(4, 2, 4)
                };
            }

            public static List<Categoria> GetCategorias()
            {
                return new List<Categoria>
                {
                    Catalogos.CreateCategoriaInternacional(),
                    Catalogos.CreateCategoriaNacional(),
                    new Categoria { Id = 3, Nombre = "Selecciones", Descripcion = "Selecciones nacionales", Activo = true, FechaCreacion = DateTime.Now }
                };
            }

            public static List<EstadoArticulo> GetEstadosArticulo()
            {
                return new List<EstadoArticulo>
                {
                    Catalogos.CreateEstadoDisponible(),
                    Catalogos.CreateEstadoPrestado(),
                    new EstadoArticulo { Id = 3, Nombre = "Mantenimiento", Descripcion = "En mantenimiento", Activo = true, FechaCreacion = DateTime.Now }
                };
            }

            public static List<EstadoPrestamo> GetEstadosPrestamo()
            {
                return new List<EstadoPrestamo>
                {
                    Catalogos.CreateEstadoPendiente(),
                    Catalogos.CreateEstadoAprobado(),
                    new EstadoPrestamo { Id = 3, Nombre = "Entregado", Descripcion = "Entregado", Activo = true, FechaCreacion = DateTime.Now },
                    new EstadoPrestamo { Id = 4, Nombre = "Devuelto", Descripcion = "Devuelto", Activo = true, FechaCreacion = DateTime.Now }
                };
            }
        }
    }
}
