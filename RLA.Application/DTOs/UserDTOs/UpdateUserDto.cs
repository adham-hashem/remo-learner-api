using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.UserDTOs
{
    /// <summary>
    /// DTO for updating user information.
    /// </summary>
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

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