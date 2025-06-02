using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace ELP.Domain.Entities
{
    public class StudentCourse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        //[Required]
        public Guid? SectionId { get; set; }

        [ForeignKey(nameof(SectionId))]
        public Section Section { get; set; } = null!;

        [Required]
        public Guid TermId { get; set; }

        [ForeignKey(nameof(TermId))]
        public Term Term { get; set; } = null!;

        public double FinalGrade { get; set; } = 0;
    }
}