using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.UserDTOs
{
    /// <summary>
    /// DTO for creating a new professor user.
    /// </summary>
    public class AddProfessorDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "University ID is required.")]
        [MaxLength(20, ErrorMessage = "University ID cannot exceed 20 characters.")]
        public string UniversityId { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }
    }
}