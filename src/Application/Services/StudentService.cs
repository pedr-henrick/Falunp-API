using Application.Common;
using Application.DTOs.Student;
using Domain.Entities;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class StudentService(IStudentRepository studentRepository) : IStudentService
    {
        private readonly IStudentRepository _studentRepository = studentRepository;

        public async Task<Result<List<StudentInfoDto>>> GetAsync(StudentRequestDto studentRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                var request = studentRequestDto.Adapt<Student>();

                var students = await _studentRepository.GetAsync(request, cancellationToken);
                var response = students.Adapt<List<StudentInfoDto>>();

                return Result<List<StudentInfoDto>>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<List<StudentInfoDto>>.Failure($"Error retrieving students: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateAsync(StudentCreateDto studentDto, CancellationToken cancellationToken)
        {
            try
            {
                var studentEntity = studentDto.Adapt<Student>();
                await _studentRepository.CreateAsync(studentEntity, cancellationToken);

                return Result<string>.Success("Student added successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error retrieving students: {ex.Message}");
            }
        }
    }
}
