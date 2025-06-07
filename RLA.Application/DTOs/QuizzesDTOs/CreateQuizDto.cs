using RLA.Application.Application.DTOs.QuizQuestionsDTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.QuizzesDTOs
{
    /// <summary>
    /// DTO for creating or updating a quiz.
    /// </summary>
    public class CreateQuizDto
    {
        [Required(ErrorMessage = "Quiz title is required.")]
        [MaxLength(200, ErrorMessage = "Quiz title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Maximum score is required.")]
        [Range(1, 100, ErrorMessage = "Maximum score must be between 1 and 100.")]
        public int MaxScore { get; set; }

        [Required(ErrorMessage = "At least one question is required.")]
        public List<QuizQuestionDto> Questions { get; set; } = new List<QuizQuestionDto>();
    }
}