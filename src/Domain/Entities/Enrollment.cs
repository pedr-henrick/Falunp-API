namespace Domain.Entities
{
    public class Enrollment
    {
        public Guid? StudentId { get; set; }
        public Guid? ClassId { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Student Student { get; set; }
        public Class Class { get; set; }
    }
}
