namespace RLA.Application.DTOs
{
    /// <summary>
    /// DTO for authentication response, including JWT token and user details.
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}