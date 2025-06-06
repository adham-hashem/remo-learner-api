using System;
using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs
{
    /// <summary>
    /// DTO for submitting a single quiz answer.
    /// </summary>
    public class QuizAnswerRequest
    {
        [Required(ErrorMessage = "Question ID is required.")]
        public Guid QuestionId { get; set; }

        [Required(ErrorMessage = "Answer text is required.")]
        public string AnswerText { get; set; }
    }
}