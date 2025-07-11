using System.ComponentModel.DataAnnotations;


namespace RLA.Application.DTOs.AuthDTOs
{
    /// <summary>
    /// DTO for user login credentials.
    /// </summary>
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}