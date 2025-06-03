using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELP.Domain.Entities
{
    public class QuizAnswer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid QuizSubmissionId { get; set; }

        [ForeignKey(nameof(QuizSubmissionId))]
        public QuizSubmission QuizSubmission { get; set; } = null!;

        [Required]
        public Guid QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; } = null!;

        [Required]
        public Guid SelectedAnswerId { get; set; }

        [ForeignKey(nameof(SelectedAnswerId))]
        public Answer SelectedAnswer { get; set; } = null!;
    }
}