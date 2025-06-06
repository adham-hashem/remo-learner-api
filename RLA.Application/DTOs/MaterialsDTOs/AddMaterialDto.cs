using System.ComponentModel.DataAnnotations;

namespace RLA.Application.DTOs.MaterialsDTOs
{
    /// <summary>
    /// DTO for adding or updating course material.
    /// </summary>
    public class AddMaterialDto
    {
        [Required(ErrorMessage = "Week number is required.")]
        [Range(1, 16, ErrorMessage = "Week number must be between 1 and 16.")]
        public int WeekNumber { get; set; }

        [Required(ErrorMessage = "Lecture title is required.")]
        [MaxLength(200, ErrorMessage = "Lecture title cannot exceed 200 characters.")]
        public string LectureTitle { get; set; }

        [MaxLength(1000, ErrorMessage = "Lecture description cannot exceed 1000 characters.")]
        public string LectureDescription { get; set; }

        [Required(ErrorMessage = "File path is required.")]
        [MaxLength(500, ErrorMessage = "File path cannot exceed 500 characters.")]
        public string FilePath { get; set; }
    }
}