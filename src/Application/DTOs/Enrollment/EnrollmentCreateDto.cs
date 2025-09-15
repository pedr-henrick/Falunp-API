namespace Application.DTOs.Enrollment
{
    public class EnrollmentCreateDto
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
