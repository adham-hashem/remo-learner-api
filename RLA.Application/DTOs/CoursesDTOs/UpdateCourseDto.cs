using System;
using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.CoursesDTOs
{
    /// <summary>
    /// DTO for updating an existing course.
    /// </summary>
    public class UpdateCourseDto
    {
        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Course code is required.")]
        [MaxLength(10, ErrorMessage = "Course code cannot exceed 10 characters.")]
        public string Code { get; set; }

        [MaxLength(500, ErrorMessage = "Overview cannot exceed 500 characters.")]
        public string Overview { get; set; }

        [Required(ErrorMessage = "Day of week is required.")]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        public string Time { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [MaxLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; }

        [Range(1, 6, ErrorMessage = "Credit hours must be between 1 and 6.")]
        public int CreditHours { get; set; }

        [Required(ErrorMessage = "Level ID is required.")]
        public Guid LevelId { get; set; }

        [Required(ErrorMessage = "Professor ID is required.")]
        public Guid ProfessorId { get; set; }

        [Required(ErrorMessage = "Term ID is required.")]
        public Guid TermId { get; set; }
    }
}