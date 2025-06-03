using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELP.Domain.Entities
{
    public class QuizSubmission
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required]
        public Guid QuizId { get; set; }

        [ForeignKey(nameof(QuizId))]
        public Quiz Quiz { get; set; } = null!;

        [Required]
        public double Score { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; }

        public ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
    }
}