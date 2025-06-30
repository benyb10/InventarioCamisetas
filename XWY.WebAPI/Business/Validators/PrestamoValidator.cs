using FluentValidation;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Validators
{
    public class PrestamoCreateValidator : AbstractValidator<PrestamoCreateDto>
    {
        public PrestamoCreateValidator()
    {
        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido");

        RuleFor(x => x.ArticuloId)
            .GreaterThan(0).WithMessage("Debe seleccionar un artículo válido");

        RuleFor(x => x.FechaEntregaEstimada)
            .NotEmpty().WithMessage("La fecha de entrega estimada es obligatoria")
            .GreaterThan(DateTime.Now.Date).WithMessage("La fecha de entrega debe ser posterior a hoy")
            .LessThan(DateTime.Now.AddDays(90)).WithMessage("La fecha de entrega no puede ser superior a 90 días");

        RuleFor(x => x.FechaDevolucionEstimada)
            .GreaterThan(x => x.FechaEntregaEstimada).WithMessage("La fecha de devolución debe ser posterior a la fecha de entrega")
            .LessThan(DateTime.Now.AddDays(180)).WithMessage("La fecha de devolución no puede ser superior a 180 días")
            .When(x => x.FechaDevolucionEstimada.HasValue);

        RuleFor(x => x.Observaciones)
            .MaximumLength(500).WithMessage("Las observaciones no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observaciones));
    }
}

public class PrestamoUpdateValidator : AbstractValidator<PrestamoUpdateDto>
{
    public PrestamoUpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

        RuleFor(x => x.FechaEntregaEstimada)
            .NotEmpty().WithMessage("La fecha de entrega estimada es obligatoria")
            .GreaterThan(DateTime.Now.Date.AddDays(-1)).WithMessage("La fecha de entrega no puede ser anterior a ayer");

        RuleFor(x => x.FechaDevolucionEstimada)
            .GreaterThan(x => x.FechaEntregaEstimada).WithMessage("La fecha de devolución debe ser posterior a la fecha de entrega")
            .When(x => x.FechaDevolucionEstimada.HasValue);

        RuleFor(x => x.Observaciones)
            .MaximumLength(500).WithMessage("Las observaciones no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observaciones));
    }
}

public class PrestamoAprobacionValidator : AbstractValidator<PrestamoAprobacionDto>
{
    public PrestamoAprobacionValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

        RuleFor(x => x.AprobadoPor)
            .GreaterThan(0).WithMessage("Debe especificar quién aprueba el préstamo");

        RuleFor(x => x.ObservacionesAprobacion)
            .MaximumLength(500).WithMessage("Las observaciones de aprobación no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ObservacionesAprobacion));

        RuleFor(x => x.FechaEntregaReal)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de entrega real no puede ser futura")
            .GreaterThan(DateTime.Now.AddDays(-30)).WithMessage("La fecha de entrega real no puede ser anterior a 30 días")
            .When(x => x.FechaEntregaReal.HasValue);
    }
}

public class PrestamoDevolucionValidator : AbstractValidator<PrestamoDevolucionDto>
{
    public PrestamoDevolucionValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

        RuleFor(x => x.FechaDevolucionReal)
            .NotEmpty().WithMessage("La fecha de devolución real es obligatoria")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de devolución real no puede ser futura")
            .GreaterThan(DateTime.Now.AddDays(-365)).WithMessage("La fecha de devolución real no puede ser anterior a un año");

        RuleFor(x => x.ObservacionesDevolucion)
            .MaximumLength(500).WithMessage("Las observaciones de devolución no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ObservacionesDevolucion));
    }
}

public class PrestamoFiltroValidator : AbstractValidator<PrestamoFiltroDto>
{
    public PrestamoFiltroValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("La página debe ser mayor a 0");

        RuleFor(x => x.RegistrosPorPagina)
            .GreaterThan(0).WithMessage("Los registros por página deben ser mayor a 0")
            .LessThanOrEqualTo(100).WithMessage("No se pueden mostrar más de 100 registros por página");

        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("El usuario debe ser válido")
            .When(x => x.UsuarioId.HasValue);

        RuleFor(x => x.ArticuloId)
            .GreaterThan(0).WithMessage("El artículo debe ser válido")
            .When(x => x.ArticuloId.HasValue);

        RuleFor(x => x.EstadoPrestamoId)
            .GreaterThan(0).WithMessage("El estado debe ser válido")
            .When(x => x.EstadoPrestamoId.HasValue);

        RuleFor(x => x.FechaHasta)
            .GreaterThanOrEqualTo(x => x.FechaDesde).WithMessage("La fecha hasta debe ser posterior o igual a la fecha desde")
            .When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue);
    }
}
}
