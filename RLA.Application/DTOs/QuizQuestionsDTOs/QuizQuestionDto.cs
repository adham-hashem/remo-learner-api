using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.QuizQuestionsDTOs
{
    /// <summary>
    /// DTO for a quiz question, including options and correct answer.
    /// </summary>
    public class QuizQuestionDto
    {
        [Required(ErrorMessage = "Question text is required.")]
        [MaxLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
        public string Text { get; set; }

        [Required(ErrorMessage = "At least one option is required.")]
        [MinLength(2, ErrorMessage = "At least two options are required.")]
        public List<string> Options { get; set; } = new List<string>();

        [Required(ErrorMessage = "Correct answer is required.")]
        public string CorrectAnswer { get; set; }
    }
}