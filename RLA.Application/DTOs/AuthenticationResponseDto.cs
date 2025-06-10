namespace RLA.Application.DTOs
{
    /// <summary>
    /// DTO for authentication response, including JWT token and user details.
    /// </summary>
    public class AuthenticationResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}