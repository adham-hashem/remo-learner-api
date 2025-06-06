namespace ELP.Application.DTOs
{
    /// <summary>
    /// DTO for authentication response, including JWT token and user details.
    /// </summary>
    public class AuthenticationResponseDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}