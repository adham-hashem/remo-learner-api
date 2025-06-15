using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RLA.Application.DTOs.UserDTOs
{
    /// <summary>
    /// DTO for creating a new professor user.
    /// </summary>
    public class AddProfessorDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "University ID is required.")]
        [MaxLength(20, ErrorMessage = "University ID cannot exceed 20 characters.")]
        public string UniversityId { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be a 14-digit number.")]
        public string NationalId { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;
    }
}