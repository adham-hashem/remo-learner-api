using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RLA.Domain.Entities
{
    public class CourseDiscussion
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        [Required]
        public Guid ProfessorId { get; set; }

        [ForeignKey(nameof(ProfessorId))]
        public Professor Professor { get; set; } = null!;

        [Required, MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    }
}