using FluentValidation;
using XWY.WebAPI.Business.DTOs;

namespace XWY.WebAPI.Business.Validators
{
    public class ArticuloCreateValidator : AbstractValidator<ArticuloCreateDto>
    {
        public ArticuloCreateValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es obligatorio")
                .MaximumLength(20).WithMessage("El código no puede exceder 20 caracteres")
                .Matches("^[A-Z0-9\\-]+$").WithMessage("El código solo puede contener letras mayúsculas, números y guiones");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Equipo)
                .NotEmpty().WithMessage("El equipo es obligatorio")
                .MaximumLength(100).WithMessage("El equipo no puede exceder 100 caracteres");

            RuleFor(x => x.Temporada)
                .MaximumLength(20).WithMessage("La temporada no puede exceder 20 caracteres")
                .Matches("^[0-9\\-/]+$").WithMessage("La temporada debe tener formato válido (ej: 2023-2024)")
                .When(x => !string.IsNullOrEmpty(x.Temporada));

            RuleFor(x => x.Talla)
                .MaximumLength(10).WithMessage("La talla no puede exceder 10 caracteres")
                .Must(BeValidSize).WithMessage("La talla debe ser válida (XS, S, M, L, XL, XXL)")
                .When(x => !string.IsNullOrEmpty(x.Talla));

            RuleFor(x => x.Color)
                .MaximumLength(50).WithMessage("El color no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Color));

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
                .LessThan(10000).WithMessage("El precio no puede exceder $10,000")
                .When(x => x.Precio.HasValue);

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida");

            RuleFor(x => x.EstadoArticuloId)
                .GreaterThan(0).WithMessage("Debe seleccionar un estado válido");

            RuleFor(x => x.Ubicacion)
                .MaximumLength(100).WithMessage("La ubicación no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Ubicacion));

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo")
                .LessThan(1000).WithMessage("El stock no puede exceder 999 unidades");
        }

        private bool BeValidSize(string size)
        {
            var validSizes = new[] { "XS", "S", "M", "L", "XL", "XXL", "XXXL" };
            return validSizes.Contains(size?.ToUpper());
        }
    }

    public class ArticuloUpdateValidator : AbstractValidator<ArticuloUpdateDto>
    {
        public ArticuloUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es obligatorio")
                .MaximumLength(20).WithMessage("El código no puede exceder 20 caracteres")
                .Matches("^[A-Z0-9\\-]+$").WithMessage("El código solo puede contener letras mayúsculas, números y guiones");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Equipo)
                .NotEmpty().WithMessage("El equipo es obligatorio")
                .MaximumLength(100).WithMessage("El equipo no puede exceder 100 caracteres");

            RuleFor(x => x.Temporada)
                .MaximumLength(20).WithMessage("La temporada no puede exceder 20 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Temporada));

            RuleFor(x => x.Talla)
                .MaximumLength(10).WithMessage("La talla no puede exceder 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Talla));

            RuleFor(x => x.Color)
                .MaximumLength(50).WithMessage("El color no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Color));

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
                .LessThan(10000).WithMessage("El precio no puede exceder $10,000")
                .When(x => x.Precio.HasValue);

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida");

            RuleFor(x => x.EstadoArticuloId)
                .GreaterThan(0).WithMessage("Debe seleccionar un estado válido");

            RuleFor(x => x.Ubicacion)
                .MaximumLength(100).WithMessage("La ubicación no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Ubicacion));

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo")
                .LessThan(1000).WithMessage("El stock no puede exceder 999 unidades");
        }
    }

    public class ArticuloFiltroValidator : AbstractValidator<ArticuloFiltroDto>
    {
        public ArticuloFiltroValidator()
        {
            RuleFor(x => x.Pagina)
                .GreaterThan(0).WithMessage("La página debe ser mayor a 0");

            RuleFor(x => x.RegistrosPorPagina)
                .GreaterThan(0).WithMessage("Los registros por página deben ser mayor a 0")
                .LessThanOrEqualTo(100).WithMessage("No se pueden mostrar más de 100 registros por página");

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("La categoría debe ser válida")
                .When(x => x.CategoriaId.HasValue);

            RuleFor(x => x.EstadoArticuloId)
                .GreaterThan(0).WithMessage("El estado debe ser válido")
                .When(x => x.EstadoArticuloId.HasValue);
        }
    }
}
