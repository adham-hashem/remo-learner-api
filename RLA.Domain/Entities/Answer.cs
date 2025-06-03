using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RLA.Domain.Entities
{
    public class Answer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; } = null!;

        public ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
    }
}