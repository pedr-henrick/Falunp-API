using Application.DTOs.Class;
using FluentValidation;

namespace Application.Validators
{
    public class ClassDtoValidator : AbstractValidator<ClassDto>
    {
        public ClassDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória");
        }
    }
}
