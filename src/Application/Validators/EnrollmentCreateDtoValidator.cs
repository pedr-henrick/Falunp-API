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
                .NotEmpty().WithMessage("ClassId is required.")
                .MustAsync(async (id, cancellation) => (await _studentRepository.GetByIdAsync(id, cancellation)) != null)
                .WithMessage("Student not found.");

            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("ClassId is required.")
                .MustAsync(async (id, cancellation) => (await _classRepository.GetAsync(new Class() { Id = id }, cancellation)) != null)
                .WithMessage("Class not found.");

            RuleFor(x => x.RegistrationDate)
                .NotEmpty().WithMessage("RegistrationDate is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("RegistrationDate cannot be in the future.");
        }
    }
}
