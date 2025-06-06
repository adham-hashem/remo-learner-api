using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RLA.Application.DTOs;

namespace RLA.Application.DTOs
{
    /// <summary>
    /// DTO for submitting a complete quiz with multiple answers.
    /// </summary>
    public class QuizSubmissionRequest
    {
        [Required(ErrorMessage = "Quiz ID is required.")]
        public Guid QuizId { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "At least one answer is required.")]
        public List<QuizAnswerRequest> Answers { get; set; } = new List<QuizAnswerRequest>();
    }
}