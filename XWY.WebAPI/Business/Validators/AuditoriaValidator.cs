using FluentValidation;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Validators
{
    public class AuditoriaLogCreateValidator : AbstractValidator<AuditoriaLogCreateDto>
    {
        public AuditoriaLogCreateValidator()
        {
            RuleFor(x => x.Accion)
                .NotEmpty().WithMessage("La acción es obligatoria")
                .MaximumLength(100).WithMessage("La acción no puede exceder 100 caracteres")
                .Must(BeValidAction).WithMessage("La acción debe ser válida (CREATE, UPDATE, DELETE, LOGIN, LOGOUT, EXPORT)");

            RuleFor(x => x.Tabla)
                .NotEmpty().WithMessage("La tabla es obligatoria")
                .MaximumLength(50).WithMessage("La tabla no puede exceder 50 caracteres");

            RuleFor(x => x.RegistroId)
                .GreaterThan(0).WithMessage("El ID del registro debe ser mayor a 0")
                .When(x => x.RegistroId.HasValue);

            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("El ID del usuario debe ser mayor a 0")
                .When(x => x.UsuarioId.HasValue);

            RuleFor(x => x.DireccionIP)
                .MaximumLength(45).WithMessage("La dirección IP no puede exceder 45 caracteres")
                .Matches(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$")
                .WithMessage("La dirección IP no tiene un formato válido")
                .When(x => !string.IsNullOrEmpty(x.DireccionIP));

            RuleFor(x => x.UserAgent)
                .MaximumLength(500).WithMessage("El User Agent no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.UserAgent));
        }

        private bool BeValidAction(string accion)
        {
            if (string.IsNullOrEmpty(accion))
                return false;

            var validActions = new[] { "CREATE", "UPDATE", "DELETE", "LOGIN", "LOGOUT", "EXPORT", "APPROVE", "REJECT", "DELIVER", "RETURN" };
            return validActions.Contains(accion.ToUpper());
        }
    }

    public class AuditoriaLogFiltroValidator : AbstractValidator<AuditoriaLogFiltroDto>
    {
        public AuditoriaLogFiltroValidator()
        {
            RuleFor(x => x.Pagina)
                .GreaterThan(0).WithMessage("La página debe ser mayor a 0");

            RuleFor(x => x.RegistrosPorPagina)
                .GreaterThan(0).WithMessage("Los registros por página deben ser mayor a 0")
                .LessThanOrEqualTo(100).WithMessage("No se pueden mostrar más de 100 registros por página");

            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("El usuario debe ser válido")
                .When(x => x.UsuarioId.HasValue);

            RuleFor(x => x.Accion)
                .MaximumLength(100).WithMessage("La acción no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Accion));

            RuleFor(x => x.Tabla)
                .MaximumLength(50).WithMessage("La tabla no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Tabla));

            RuleFor(x => x.FechaHasta)
                .GreaterThanOrEqualTo(x => x.FechaDesde).WithMessage("La fecha hasta debe ser posterior o igual a la fecha desde")
                .When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue);

            RuleFor(x => x.FechaDesde)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha desde no puede ser futura")
                .When(x => x.FechaDesde.HasValue);

            RuleFor(x => x.FechaHasta)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha hasta no puede ser futura")
                .When(x => x.FechaHasta.HasValue);
        }
    }
}
