using Application.Common;
using Application.DTOs.Enrollment;

namespace Application.Services
{
    public interface IEnrollmentService
    {
        Task<Result<List<EnrollmentInfoDto>>> GetAsync(EnrollmentInfoDto enrollmentRequestDto, CancellationToken cancellationToken);
        Task<Result<string>> CreateAsync(EnrollmentCreateDto enrollmentCreatetDto, CancellationToken cancellationToken);
        Task<Result<string>> UpdateAsync(EnrollmentUpdateDto enrollmentUpdateDto, CancellationToken cancellationToken);
        Task<Result<string>> DeleteAsync(EnrollmentFilterDto enrollmentFilterDto, CancellationToken cancellationToken);
    }
}
