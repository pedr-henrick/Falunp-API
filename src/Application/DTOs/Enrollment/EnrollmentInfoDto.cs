namespace Application.DTOs.Enrollment
{
    public class EnrollmentInfoDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
