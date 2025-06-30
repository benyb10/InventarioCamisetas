using AutoMapper;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;
using XWY.WebAPI.Entities;

namespace XWY.WebAPI.Business.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ResponseDto<PagedResponseDto<UsuarioDto>>> GetAllPagedAsync(int pagina, int registrosPorPagina)
        {
            try
            {
                var totalRegistros = await _unitOfWork.Usuarios.GetTotalActiveUsersAsync();
                var usuarios = await _unitOfWork.Usuarios.GetPagedAsync(pagina, registrosPorPagina,
                    u => u.Activo, u => u.Nombres);

                var usuariosDto = new List<UsuarioDto>();
                foreach (var usuario in usuarios)
                {
                    var usuarioConRol = await _unitOfWork.Usuarios.GetByIdWithRolAsync(usuario.Id);
                    usuariosDto.Add(_mapper.Map<UsuarioDto>(usuarioConRol));
                }

                var pagedResponse = new PagedResponseDto<UsuarioDto>(usuariosDto, totalRegistros, pagina, registrosPorPagina);
                return new ResponseDto<PagedResponseDto<UsuarioDto>>(pagedResponse);
            }
            catch (Exception ex)
            {
                return new ResponseDto<PagedResponseDto<UsuarioDto>>($"Error al obtener usuarios: {ex.Message}");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> GetByIdAsync(int id)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdWithRolAsync(id);
                if (usuario == null || !usuario.Activo)
                {
                    return new ResponseDto<UsuarioDto>("Usuario no encontrado");
                }

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
                return new ResponseDto<UsuarioDto>(usuarioDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<UsuarioDto>($"Error al obtener usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> GetByEmailAsync(string email)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByEmailWithRolAsync(email);
                if (usuario == null)
                {
                    return new ResponseDto<UsuarioDto>("Usuario no encontrado");
                }

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
                return new ResponseDto<UsuarioDto>(usuarioDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<UsuarioDto>($"Error al obtener usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> CreateAsync(UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                if (await _unitOfWork.Usuarios.ExistsByEmailAsync(usuarioCreateDto.Email))
                {
                    return new ResponseDto<UsuarioDto>("El email ya está registrado");
                }

                if (await _unitOfWork.Usuarios.ExistsByCedulaAsync(usuarioCreateDto.Cedula))
                {
                    return new ResponseDto<UsuarioDto>("La cédula ya está registrada");
                }

                var usuario = _mapper.Map<Usuario>(usuarioCreateDto);
                usuario.PasswordHash = _authService.HashPassword(usuarioCreateDto.Password);
                usuario.FechaCreacion = DateTime.Now;

                await _unitOfWork.Usuarios.AddAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                var usuarioConRol = await _unitOfWork.Usuarios.GetByIdWithRolAsync(usuario.Id);
                var usuarioDto = _mapper.Map<UsuarioDto>(usuarioConRol);

                return new ResponseDto<UsuarioDto>(usuarioDto, "Usuario creado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<UsuarioDto>($"Error al crear usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<UsuarioDto>> UpdateAsync(UsuarioUpdateDto usuarioUpdateDto)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioUpdateDto.Id);
                if (usuario == null)
                {
                    return new ResponseDto<UsuarioDto>("Usuario no encontrado");
                }

                if (await _unitOfWork.Usuarios.ExistsByEmailAsync(usuarioUpdateDto.Email) &&
                    usuario.Email != usuarioUpdateDto.Email)
                {
                    return new ResponseDto<UsuarioDto>("El email ya está registrado");
                }

                if (await _unitOfWork.Usuarios.ExistsByCedulaAsync(usuarioUpdateDto.Cedula) &&
                    usuario.Cedula != usuarioUpdateDto.Cedula)
                {
                    return new ResponseDto<UsuarioDto>("La cédula ya está registrada");
                }

                usuario.Cedula = usuarioUpdateDto.Cedula;
                usuario.Nombres = usuarioUpdateDto.Nombres;
                usuario.Apellidos = usuarioUpdateDto.Apellidos;
                usuario.Email = usuarioUpdateDto.Email;
                usuario.Telefono = usuarioUpdateDto.Telefono;
                usuario.RolId = usuarioUpdateDto.RolId;
                usuario.Activo = usuarioUpdateDto.Activo;

                _unitOfWork.Usuarios.Update(usuario);
                await _unitOfWork.SaveChangesAsync();

                var usuarioConRol = await _unitOfWork.Usuarios.GetByIdWithRolAsync(usuario.Id);
                var usuarioDto = _mapper.Map<UsuarioDto>(usuarioConRol);

                return new ResponseDto<UsuarioDto>(usuarioDto, "Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<UsuarioDto>($"Error al actualizar usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
                if (usuario == null)
                {
                    return new ResponseDto<bool>("Usuario no encontrado");
                }

                usuario.Activo = false;
                _unitOfWork.Usuarios.Update(usuario);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al eliminar usuario: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<UsuarioDto>>> GetByRolAsync(int rolId)
        {
            try
            {
                var usuarios = await _unitOfWork.Usuarios.GetByRolAsync(rolId);
                var usuariosDto = _mapper.Map<List<UsuarioDto>>(usuarios);
                return new ResponseDto<List<UsuarioDto>>(usuariosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<UsuarioDto>>($"Error al obtener usuarios por rol: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<UsuarioDto>>> SearchAsync(string searchTerm)
        {
            try
            {
                var usuarios = await _unitOfWork.Usuarios.SearchUsersAsync(searchTerm);
                var usuariosDto = _mapper.Map<List<UsuarioDto>>(usuarios);
                return new ResponseDto<List<UsuarioDto>>(usuariosDto);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<UsuarioDto>>($"Error al buscar usuarios: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ExistsByEmailAsync(string email)
        {
            try
            {
                var exists = await _unitOfWork.Usuarios.ExistsByEmailAsync(email);
                return new ResponseDto<bool>(exists);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al verificar email: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ExistsByCedulaAsync(string cedula)
        {
            try
            {
                var exists = await _unitOfWork.Usuarios.ExistsByCedulaAsync(cedula);
                return new ResponseDto<bool>(exists);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>($"Error al verificar cédula: {ex.Message}");
            }
        }

        public async Task<ResponseDto<int>> GetTotalActiveUsersAsync()
        {
            try
            {
                var total = await _unitOfWork.Usuarios.GetTotalActiveUsersAsync();
                return new ResponseDto<int>(total);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>($"Error al obtener total de usuarios: {ex.Message}");
            }
        }
    }
}
