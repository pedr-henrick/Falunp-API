using Application.DTOs.Enrollment;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validators
{
    public class EnrollmentCreateDtoValidator : AbstractValidator<EnrollmentCreateDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;

        public EnrollmentCreateDtoValidator(IStudentRepository studentRepository, IClassRepository classRepository)
        {
            _classRepository = classRepository;
            _studentRepository = studentRepository;

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("ClassId é obrigatório.")
                .MustAsync(async (id, cancellation) => (await _studentRepository.GetAsync(new Student() { Id = id }, cancellation)) == null)
                .WithMessage("Aluno não encontrado.");

            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("ClassId é obrigatório.")
                .MustAsync(async (id, cancellation) => (await _classRepository.GetAsync(new Class() { Id = id}, cancellation)) == null)
                .WithMessage("Turma não encontrada.");

            RuleFor(x => x.RegistrationDate)
                .NotEmpty().WithMessage("RegistrationDate é obrigatório.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Data de matrícula não pode ser futura.");
        }
    } 
}
