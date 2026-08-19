namespace IdentityService.Features.Auth.DTOs.Responses
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string Role { get; set; } = "";

        public string AccessToken { get; set; } = "";

        public string RefreshToken { get; set; } = "";
    }
}
