using Application.DTOs.Student;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validators
{
    public class StudentUpdateDtoValidator : AbstractValidator<StudentUpdateDto>
    {
        private readonly IStudentRepository _studentRepository;

        public StudentUpdateDtoValidator(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Data de nascimento é obrigatória")
                .LessThan(DateTime.Now.AddYears(-14)).WithMessage("Aluno deve ter pelo menos 14 anos")
                .GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("Data de nascimento inválida");

            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Length(11).WithMessage("CPF deve ter exatamente 11 dígitos")
                .Must(BeValidCpf).WithMessage("CPF inválido");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ser válido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");
        }

        private bool BeValidCpf(string cpf)
        {
            // Remover caracteres não numéricos
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
                return false;

            int[] multipliers1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multipliers2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf[..9];
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers1[i];

            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            tempCpf += digit1;
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers2[i];

            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf.EndsWith($"{digit1}{digit2}");
        }
    }
}
