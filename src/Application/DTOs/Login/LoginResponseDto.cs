namespace Application.DTOs.Login
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserInfoDto User { get; set; }
    }
}
