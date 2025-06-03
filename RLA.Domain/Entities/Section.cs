using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RLA.Domain.Entities
{
    public class Section
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Time { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        [Required]
        public Guid AssistantId { get; set; }

        [ForeignKey(nameof(AssistantId))]
        public Assistant Assistant { get; set; } = null!;

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}