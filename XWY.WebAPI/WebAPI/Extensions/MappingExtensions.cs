using AutoMapper;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.WebAPI.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = config.CreateMapper();
            return mapper.Map<TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = config.CreateMapper();
            return mapper.Map(source, destination);
        }

        public static List<TDestination> MapToList<TDestination>(this IEnumerable<object> source)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = config.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
        }
    }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol.Nombre));

            CreateMap<UsuarioCreateDto, Usuario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaUltimoAcceso, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Rol, opt => opt.Ignore())
                .ForMember(dest => dest.Prestamos, opt => opt.Ignore())
                .ForMember(dest => dest.PrestamosAprobados, opt => opt.Ignore())
                .ForMember(dest => dest.AuditoriasLog, opt => opt.Ignore());

            CreateMap<UsuarioUpdateDto, Usuario>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaUltimoAcceso, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Rol, opt => opt.Ignore())
                .ForMember(dest => dest.Prestamos, opt => opt.Ignore())
                .ForMember(dest => dest.PrestamosAprobados, opt => opt.Ignore())
                .ForMember(dest => dest.AuditoriasLog, opt => opt.Ignore());

            CreateMap<Articulo, ArticuloDto>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria.Nombre))
                .ForMember(dest => dest.EstadoArticuloNombre, opt => opt.MapFrom(src => src.EstadoArticulo.Nombre));

            CreateMap<ArticuloCreateDto, Articulo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Categoria, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoArticulo, opt => opt.Ignore())
                .ForMember(dest => dest.Prestamos, opt => opt.Ignore());

            CreateMap<ArticuloUpdateDto, Articulo>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.Categoria, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoArticulo, opt => opt.Ignore())
                .ForMember(dest => dest.Prestamos, opt => opt.Ignore());

            CreateMap<Prestamo, PrestamoDto>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombres} {src.Usuario.Apellidos}"))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.ArticuloNombre, opt => opt.MapFrom(src => src.Articulo.Nombre))
                .ForMember(dest => dest.ArticuloCodigo, opt => opt.MapFrom(src => src.Articulo.Codigo))
                .ForMember(dest => dest.EstadoPrestamoNombre, opt => opt.MapFrom(src => src.EstadoPrestamo.Nombre))
                .ForMember(dest => dest.AprobadoPorNombre, opt => opt.MapFrom(src => src.UsuarioAprobador != null ? $"{src.UsuarioAprobador.Nombres} {src.UsuarioAprobador.Apellidos}" : null));

            CreateMap<PrestamoCreateDto, Prestamo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaSolicitud, opt => opt.Ignore())
                .ForMember(dest => dest.FechaEntregaReal, opt => opt.Ignore())
                .ForMember(dest => dest.FechaDevolucionReal, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoPrestamoId, opt => opt.Ignore())
                .ForMember(dest => dest.AprobadoPor, opt => opt.Ignore())
                .ForMember(dest => dest.FechaAprobacion, opt => opt.Ignore())
                .ForMember(dest => dest.ObservacionesAprobacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Articulo, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoPrestamo, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioAprobador, opt => opt.Ignore());

            CreateMap<PrestamoUpdateDto, Prestamo>()
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.ArticuloId, opt => opt.Ignore())
                .ForMember(dest => dest.FechaSolicitud, opt => opt.Ignore())
                .ForMember(dest => dest.FechaEntregaReal, opt => opt.Ignore())
                .ForMember(dest => dest.FechaDevolucionReal, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoPrestamoId, opt => opt.Ignore())
                .ForMember(dest => dest.AprobadoPor, opt => opt.Ignore())
                .ForMember(dest => dest.FechaAprobacion, opt => opt.Ignore())
                .ForMember(dest => dest.ObservacionesAprobacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Articulo, opt => opt.Ignore())
                .ForMember(dest => dest.EstadoPrestamo, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioAprobador, opt => opt.Ignore());

            CreateMap<Rol, RolDto>();
            CreateMap<RolCreateDto, Rol>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore());

            CreateMap<RolUpdateDto, Rol>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore());

            CreateMap<Categoria, CategoriaDto>();
            CreateMap<CategoriaCreateDto, Categoria>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Articulos, opt => opt.Ignore());

            CreateMap<CategoriaUpdateDto, Categoria>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Articulos, opt => opt.Ignore());

            CreateMap<EstadoArticulo, EstadoArticuloDto>();
            CreateMap<EstadoArticuloCreateDto, EstadoArticulo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Articulos, opt => opt.Ignore());

            CreateMap<EstadoArticuloUpdateDto, EstadoArticulo>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Articulos, opt => opt.Ignore());

            CreateMap<EstadoPrestamo, EstadoPrestamoDto>();
            CreateMap<EstadoPrestamoCreateDto, EstadoPrestamo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Prestamos, opt => opt.Ignore());

            CreateMap<EstadoPrestamoUpdateDto, EstadoPrestamo>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Prestamos, opt => opt.Ignore());

            CreateMap<AuditoriaLog, AuditoriaLogDto>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? $"{src.Usuario.Nombres} {src.Usuario.Apellidos}" : null));

            CreateMap<AuditoriaLogCreateDto, AuditoriaLog>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaAccion, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());
        }
    }
}
