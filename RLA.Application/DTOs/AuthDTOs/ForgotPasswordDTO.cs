using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.AuthDTOs
{
    /// <summary>
    /// DTO for requesting a password reset via email.
    /// </summary>
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}