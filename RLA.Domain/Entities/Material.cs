using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RLA.Domain.Entities
{
    public class Material
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(20)]
        public string WeekNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LectureTitle { get; set; } = string.Empty;

        // when Type is "text"
        public string? LectureDescription { get; set; }

        // when Type is "pdf" or "image"
        [MaxLength(500)]
        public string? FilePath { get; set; }

        // Material type: "text", "pdf", "image"
        [Required]
        [MaxLength(10)]
        public string? Type { get; set; } = "text";

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;
    }
}