using Application.Common;
using Application.DTOs.Student;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class StudentService(IStudentRepository studentRepository) : IStudentService
    {
        private readonly IStudentRepository _studentRepository = studentRepository;

        public async Task<Result<List<StudentInfoDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var students = await _studentRepository.GetAllAsync(cancellationToken);
                var response = students.Adapt<List<StudentInfoDto>>();

                return Result<List<StudentInfoDto>>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<List<StudentInfoDto>>.Failure($"Error retrieving students: {ex.Message}");
            }
        }
    }
}
