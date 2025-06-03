using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELP.Domain.Entities
{
    public class Question
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public Guid QuizId { get; set; }

        [ForeignKey(nameof(QuizId))]
        public Quiz Quiz { get; set; } = null!;

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
    }
}