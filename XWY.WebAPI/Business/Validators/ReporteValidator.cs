using FluentValidation;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Validators
{
    public class ReporteParametrosValidator : AbstractValidator<ReporteParametrosDto>
    {
        public ReporteParametrosValidator()
        {
            RuleFor(x => x.TipoReporte)
                .NotEmpty().WithMessage("El tipo de reporte es obligatorio")
                .Must(BeValidReportType).WithMessage("El tipo de reporte debe ser 'articulos' o 'prestamos'");

            RuleFor(x => x.Formato)
                .NotEmpty().WithMessage("El formato es obligatorio")
                .Must(BeValidFormat).WithMessage("El formato debe ser 'pdf' o 'excel'");

            RuleFor(x => x.FechaHasta)
                .GreaterThanOrEqualTo(x => x.FechaDesde).WithMessage("La fecha hasta debe ser posterior o igual a la fecha desde")
                .When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue);

            RuleFor(x => x.FechaDesde)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha desde no puede ser futura")
                .When(x => x.FechaDesde.HasValue);

            RuleFor(x => x.FechaHasta)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha hasta no puede ser futura")
                .When(x => x.FechaHasta.HasValue);

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("La categoría debe ser válida")
                .When(x => x.CategoriaId.HasValue);

            RuleFor(x => x.EstadoArticuloId)
                .GreaterThan(0).WithMessage("El estado del artículo debe ser válido")
                .When(x => x.EstadoArticuloId.HasValue);

            RuleFor(x => x.EstadoPrestamoId)
                .GreaterThan(0).WithMessage("El estado del préstamo debe ser válido")
                .When(x => x.EstadoPrestamoId.HasValue);
        }

        private bool BeValidReportType(string tipoReporte)
        {
            if (string.IsNullOrEmpty(tipoReporte))
                return false;

            var validTypes = new[] { "articulos", "prestamos" };
            return validTypes.Contains(tipoReporte.ToLower());
        }

        private bool BeValidFormat(string formato)
        {
            if (string.IsNullOrEmpty(formato))
                return false;

            var validFormats = new[] { "pdf", "excel" };
            return validFormats.Contains(formato.ToLower());
        }
    }
}
