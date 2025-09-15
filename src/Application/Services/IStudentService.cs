using Application.Common;
using Application.DTOs.Student;

namespace Application.Services
{
    public interface IStudentService
    {
        Task<Result<List<StudentInfoDto>>> GetAsync(StudentRequestDto studentRequestDto, CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(StudentCreateDto studentDto, CancellationToken cancellationToken);
    }
}