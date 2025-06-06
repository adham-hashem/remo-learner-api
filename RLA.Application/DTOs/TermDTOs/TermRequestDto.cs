using System;
using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.TermDTOs
{
    /// <summary>
    /// DTO for creating or updating an academic term.
    /// </summary>
    public class TermRequestDto
    {
        [Required(ErrorMessage = "Term name is required.")]
        [MaxLength(50, ErrorMessage = "Term name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }
    }
}