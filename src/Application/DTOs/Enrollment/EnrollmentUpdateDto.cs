using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Enrollment
{
    public class EnrollmentUpdateDto
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
