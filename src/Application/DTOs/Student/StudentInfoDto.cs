namespace Application.DTOs.Student
{
    public class StudentInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
    }
}
