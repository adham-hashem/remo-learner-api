using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RLA.Domain.Entities
{
    public class Term
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // e.g., "Fall 2025"

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = false; // Indicates the current term

        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}