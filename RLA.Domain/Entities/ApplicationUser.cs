using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace RLA.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be a 14-digit number.")]
        public string NationalId { get; set; } = string.Empty;

        public string UniversityId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        //public Guid? LevelId { get; set; }

        //[ForeignKey(nameof(LevelId))]
        //public Level Level { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? TwoFactorSecret { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Professor> Professors { get; set; } = new List<Professor>();
        public ICollection<Assistant> Assistants { get; set; } = new List<Assistant>();
    }
}