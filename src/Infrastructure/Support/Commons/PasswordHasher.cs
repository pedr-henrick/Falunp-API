using Domain.Interfaces;

namespace Infrastructure.Support.Commons
{
    public class PasswordHasher(int workFactor = 12) : IPasswordHasher
    {
        private readonly int _workFactor = workFactor;

        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, _workFactor);

        public bool VerifyPassword(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
