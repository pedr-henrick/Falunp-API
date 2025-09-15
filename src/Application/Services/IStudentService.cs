using Application.Common;
using Application.DTOs.Student;

namespace Application.Services
{
    public interface IStudentService
    {
        Task<Result<List<StudentInfoDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(StudentCreateDto studentDto, CancellationToken cancellationToken);
    }
}