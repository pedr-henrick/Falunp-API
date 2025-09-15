using Application.DTOs.Student;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validators
{
    public class StudentCreateDtoValidator : AbstractValidator<StudentCreateDto>
    {
        private readonly IStudentRepository _studentRepository;

        public StudentCreateDtoValidator(IStudentRepository studentRepository)
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
                .Must(BeValidCpf).WithMessage("CPF inválido")
                .MustAsync(BeUniqueCpf).WithMessage("CPF já cadastrado");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ser válido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres")
                .MustAsync(BeUniqueEmail).WithMessage("Email já cadastrado");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
                .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
                .Matches(@"[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
                .Matches(@"\d").WithMessage("Senha deve conter pelo menos um número")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Senha deve conter pelo menos um símbolo especial");
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

        private async Task<bool> BeUniqueCpf(string cpf, CancellationToken cancellationToken)
        {
            return !await _studentRepository.CpfExistsAsync(cpf);
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return !await _studentRepository.EmailExistsAsync(email);
        }
    }
}
