using Application.Common;
using Application.DTOs.Student;

namespace Application.Services
{
    public interface IStudentService
    {
        Task<Result<List<StudentInfoDto>>> GetAsync(StudentFilterDto studentRequestDto, CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(StudentCreateDto studentDto, CancellationToken cancellationToken);
        Task<Result<string>> Updatesync(Guid id, StudentUpdateDto studentUpdateDto, CancellationToken cancellationToken);
        Task<Result<string>> DaleteAsync(Guid id, CancellationToken cancellationToken);
    }
}