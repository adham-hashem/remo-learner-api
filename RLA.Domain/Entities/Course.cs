using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;
using System.Reflection.Emit;

namespace ELP.Domain.Entities
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Course name must be at least 3 characters long.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Course code cannot exceed 20 characters.")]
        [RegularExpression(@"^[A-Za-z]{2,4}\d{3}$", ErrorMessage = "Course code must be 2-4 letters followed by 3 digits (e.g., CS101).")]
        public string Code { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Overview cannot exceed 500 characters.")]
        public string Overview { get; set; } = string.Empty;

        [Required(ErrorMessage = "Day of the week is required.")]
        [EnumDataType(typeof(DayOfWeek), ErrorMessage = "Invalid day of the week.")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        public TimeOnly Time { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [MaxLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        [MinLength(3, ErrorMessage = "Location must be at least 3 characters long.")]
        public string Location { get; set; } = string.Empty;

        [Range(2, 3, ErrorMessage = "The credit hours must be either 2 or 3.")]
        public int CreditHours { get; set; }

        [Required(ErrorMessage = "Level ID is required.")]
        public Guid LevelId { get; set; }

        [ForeignKey(nameof(LevelId))]
        public Level Level { get; set; } = null!;

        //[Required(ErrorMessage = "Professor ID is required.")]
        // I commented it, as when the professor is deleted, the courses are not deleted, and we set it as null
        public Guid? ProfessorId { get; set; }

        [ForeignKey(nameof(ProfessorId))]
        public Professor Professor { get; set; } = null!;

        [Required(ErrorMessage = "Term ID is required.")]
        public Guid TermId { get; set; }

        [ForeignKey(nameof(TermId))]
        public Term Term { get; set; } = null!;

        [InverseProperty(nameof(StudentCourse.Course))]
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        [InverseProperty(nameof(Quiz.Course))]
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

        [InverseProperty(nameof(Material.Course))]
        public ICollection<Material> Materials { get; set; } = new List<Material>();

        [InverseProperty(nameof(Section.Course))]
        public ICollection<Section> Sections { get; set; } = new List<Section>();

        [InverseProperty(nameof(CourseDiscussion.Course))]
        public ICollection<CourseDiscussion> Discussions { get; set; } = new List<CourseDiscussion>();
    }
}