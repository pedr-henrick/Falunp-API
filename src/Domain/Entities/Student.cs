namespace Domain.Entities
{
    public class Student
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public void SetPasswordHash(string passwordHash)
        {
            Password = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }
    }
}
